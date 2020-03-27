namespace AviaExplorer.Models.Avia
{
    public class FlightModel
    {
        public string OriginName { get; set; }

        public string DestinationName { get; set; }

        public string CombinedName => $"{ OriginName } - { DestinationName }";

        public string DestinationCountry { get; set; }

        public string OriginIATA { get; set; }

        public string DestinationIATA { get; set; }

        public string CombinedIATA => $"{ OriginIATA } - { DestinationIATA }";
    }
}