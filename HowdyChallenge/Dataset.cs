using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HowdyChallenge
{
    public class Dataset
    {
        private Answer[] answers;

        /// <summary>
        /// Can not be created directly. Use <see cref="LoadFrom(string)"/> method to create an instance
        /// </summary>
        private Dataset() { }

        /// <summary>
        /// Calculates score evaluation from current dataset on montly basis for each group of employees
        /// </summary>
        /// <param name="sessionEvaluator">Assign a score value to single employee session</param>
        /// <param name="personEvaluator">Assign a score value to multiple session scores of a single person</param>
        /// <param name="groupEvaluator">Assign a score value to based on list of scores of employees in the same group</param>
        /// <returns></returns>
        public IEnumerable<Evaluation> EvaluateDataset(Func<Answer, float> sessionEvaluator, 
            Func<IEnumerable<float>, float> personEvaluator, 
            Func<IEnumerable<float>, float> groupEvaluator)
        {
            var data = answers.Select(a => new 
                // combine single session answers into single evaluation value
                {
                    Month = new DateTime(a.AnsweredOn.Year, a.AnsweredOn.Month, 1), 
                    Group = a.GroupId, 
                    Employee = a.EmployeeId, 
                    Evaluation = sessionEvaluator(a) 
                })
                // group by month because score is evaluate on monthly basis
                .GroupBy(a => a.Month)
                .Select(mg => new
                {
                    Month = mg.Key,
                    // group by group ID because score is to be evaluated per group of employees
                    MontlyData = mg.GroupBy(g => g.Group).Select(gg => new
                    {
                        Group = gg.Key,
                        
                        // evaluate group score based on given calculation function
                        Score = groupEvaluator(
                            // group by employee to calculate score for employees with multiple sessions per month
                            gg.GroupBy(g => g.Employee).Select(eg => personEvaluator(eg.Select(a => a.Evaluation))) 
                        )
                    })
                });

            // TODO: this can be even simplyfied into single linq query
            return data.SelectMany(mg => mg.MontlyData.Select(gg => new Evaluation()
            {
                Month = mg.Month,
                Group = gg.Group,
                Value = gg.Score,
            }));
        }

        /// <summary>
        /// Create a new dataset form filename in JSON format.
        /// </summary>
        /// <param name="filename">Absolute or relative path name of the json source file</param>
        /// <returns>New dataset</returns>
        /// <exception cref="ArgumentNullException">Filename is null</exception>
        public static Dataset LoadFrom(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException(nameof(filename));

            using (FileStream fs = File.OpenRead(filename))
            {
                return new Dataset()
                {
                    answers = JsonSerializer.Deserialize<Answer[]>(fs, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })
                };
            };
        }
    }
}
