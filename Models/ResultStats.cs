namespace API.Models {
    public class ResultStats {
        public int WonByResignation { get; set; }
        public int WonByTimeout { get; set; }
        public int WonByCheckmate { get; set; }
        public int WonByAbandonment { get; set; }
        public int DrawByAgreement { get; set; }
        public int DrawByStalemate { get; set; }
        public int DrawByRepetition { get; set; }
        public int DrawByInsufficientMaterial { get; set; }
        public int DrawByTimeoutVsInsufficientMaterial { get; set; }
        public int DrawBy50Move { get; set; }
        public int LostByResignation { get; set; }
        public int LostByTimeout { get; set; }
        public int LostByCheckmate { get; set; }
        public int LostByAbandonment { get; set; }
    }
}