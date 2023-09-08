namespace HowdyChallenge
{
    public class Evaluation
    {
        /// <summary>
        /// First data of the month being evaluated
        /// </summary>
        public DateTime Month { get; set; }
        /// <summary>
        /// Group ID being evaluated
        /// </summary>
        public int Group { get; set; }
        /// <summary>
        /// Value of the group dependent on the evaluation function
        /// </summary>
        public float Value { get; set; }
    }
}
