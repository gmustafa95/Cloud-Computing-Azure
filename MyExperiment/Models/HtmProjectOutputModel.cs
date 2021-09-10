namespace MyExperiment.Models
{
    public class HtmProjectOutputModel
    {
        /// <summary>
        /// This class represents the output model of the SE project
        /// </summary>
        private string Output { get; set; }

        public HtmProjectOutputModel(string output)
        {
            Output = output;
        }
    }
}