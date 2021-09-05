namespace API.Models {
    public enum GameResultType {
        WonByResignation = 0,
        WonByTimeout,
        WonByCheckmate,
        WonByAbandonment,
        // WonByKingOfTheHill,
        // WonByThreeCheck,
        // WonByBugHouse,
        DrawByAgreement,
        DrawByStalemate,
        DrawByRepetition,
        DrawByInsufficientMaterial,
        DrawByTimeoutVsInsufficientMaterial,
        DrawBy50Move,
        LostByResignation,
        LostByTimeout,
        LostByCheckmate,
        LostByAbandonment,
        // LostByThreeCheck,
        // LostByKingOfTheHill,
        // LostByBugHousePartnerLose
    };
}