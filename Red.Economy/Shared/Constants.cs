namespace Red.Economy
{
    public static class EconomyEvents
    {
        public const string EVENT_C_ACCOUNT_CREATE = "Economy:Client:createAccount";
        public const string EVENT_C_ACCOUNT_DELETE = "Economy:Client:deleteAccount";
        public const string EVENT_C_ACCOUNT_EDIT = "Economy:Client:editAccount";
        public const string EVENT_C_ACCOUNT_FREEEZE = "Economy:Client:freezeAccount";
        public const string EVENT_C_TRANSFER_SUCCESS = "Economy:Client:transferSuccess";
        public const string EVENT_C_CHANGE_CASH = "Economy:Client:changeCash";
        public const string EVENT_C_TOGGLE_HUD = "Economy:Client:toggleHud";
        public const string EVENT_C_PEEK_HUD = "Economy:Client:peekHud";
        public const string EVENT_S_GIVE_CASH = "Economy:Server:giveCash";
    }

    public class EconomyLocations
    {
        // Type = 0 - Unowned ATM (can be used with any bank account)
        // Type = 1 - Fleeca ATM (requires Fleeca Bank Account)
        // Type = 2 - Maze Bank ATM (requires Maze Bank Account)

        public static EconomyLocation[] economyLocations =
        [
            new EconomyLocation { LocationId = 1, Position = new(-113.39f, 6469.93f, 31.63f), Type = 1, IsATM = false, DisplayBlip = true },
            new EconomyLocation { LocationId = 2, Position = new(-95.02f, 6456.69f, 31.46f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 3, Position = new(-96.98f, 6455.04f, 31.46f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 4, Position = new(174.40f, 6637.71f, 31.57f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 5, Position = new(155.37f, 6642.40f, 31.62f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 6, Position = new(1735.40f, 6410.97f, 35.04f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 7, Position = new(-283.45f, 6225.91f, 31.49f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 8, Position = new(-387.10f, 6045.79f, 31.50f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 9, Position = new(-133.72f, 6366.06f, 31.48f), Type = 0, IsATM = true, DisplayBlip = true },

            new EconomyLocation { LocationId = 2, Position = new(-95.02f, 6456.69f, 31.46f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 2, Position = new(-95.02f, 6456.69f, 31.46f), Type = 0, IsATM = true, DisplayBlip = true },
            new EconomyLocation { LocationId = 2, Position = new(-95.02f, 6456.69f, 31.46f), Type = 0, IsATM = true, DisplayBlip = true },
        ];
    }
}