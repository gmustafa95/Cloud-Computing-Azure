namespace MyExperiment.Models
{
   /// <summary>
   /// This class represents the input data model of SE project
   /// </summary>
    public class HtmProjectInputDataModel
    {
        public int NumberOfCells { get; set; }
        public int NumberOfIterations { get; set; }

        public int MaxValue { get; set; }

        public int MinValue { get; set; }

        public int ColumnDimensions { get; set; }

    }
}