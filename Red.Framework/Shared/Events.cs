namespace Red.Framework
{
    public static class Events
    {
        #region Client Events
        public const string ClientChangeCharacter = "Framework:Client:changeCharacter";
        public const string ClientCreateCharacter = "Framework:Client:createCharacter";
        public const string ClientDeleteCharacter = "Framework:Client:deleteCharacter";
        public const string ClientEditCharacter = "Framework:Client:editCharacter";
        public const string ClientReturnCharacters = "Framework:Client:returnCharacters";
        public const string ClientSelectCharacter = "Framework:Client:selectCharacter";
        public const string ClientSelectedCharacter = "Framework:Client:selectedCharacter";
        public const string ClientUpdateAreaOfPatrol = "Framework:Client:updateAOP";
        public const string ClientSyncInformation = "Framework:Client:syncInformation";
        public const string ClientAreaOfPatrolErrorNotify = "Framework:Client:aopErrorNotify";
        #endregion

        #region Server Events
        public const string ServerCreateCharacter = "Framework:Server:createCharacter";
        public const string ServerDeleteCharacter = "Framework:Server:deleteCharacter";
        public const string ServerEditCharacter = "Framework:Server:editCharacter";
        public const string ServerGetCharacter = "Framework:Server:getCharacter";
        public const string ServerSyncInformation = "Framework:Server:syncInformation";
        public const string ServerDropUserFromFramework = "Framework:Server:dropUserFromFramework";
        #endregion

        #region Base Game Events
        public const string PlayerSpawned = "playerSpawned";
        public const string PlayerConnecting = "playerConnecting";
        #endregion
    }
}