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
            // group by month (and group) because score is evaluate on monthly basis
            .GroupBy(a => new { a.Month, a.Group })
            .Select(g => new Evaluation()
            {
                Month = g.Key.Month,
                Group = g.Key.Group,
                // group evaluator will calculate group score for this month
                Value = groupEvaluator(
                    // group score is evaluated taking each employee into consideration only once
                    g.GroupBy(gg => gg.Employee).Select(eg => 
                        // if employee answered multiple times in given month, single value is calculated
                        personEvaluator(eg.Select(a => a.Evaluation))
                    )
                )
            });

            return data;
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
