namespace HowdyChallenge
{
    internal class Program
    {
        private static void EvaluateDataset(Dataset dataset)
        {
            // YOU CAN CUSTOMIZE EVALUATION FUNCTIONS HERE //

            var evaluations = dataset.EvaluateDataset(
                // evaluation of a single session of an employee is a simple sum of all answers
                a => a.Answer1 + a.Answer2 + a.Answer3 + a.Answer4 + a.Answer5,
                // if multiple answers of an employee per month are included, we consider the maximum value
                pa => pa.Max(),
                // group evaluation is calculated as an averege of all empoloyee's evaluation
                ga => ga.Average()
            );

            foreach (Evaluation eval in evaluations.OrderBy(e => e.Month).ThenBy(e => e.Group))
            {
                Console.WriteLine($"Evaluation for month: {eval.Month:M}, group: {eval.Group}, value: {eval.Value}");
            }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: HowdyChallenge.exe [Filename]");
                return;
            }

            try
            {
                string filename = args[0];
                Dataset dataset = Dataset.LoadFrom(filename);

                EvaluateDataset(dataset);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}