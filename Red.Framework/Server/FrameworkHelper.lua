local roleIds = {
    ['ROLE_ID'] = 'LSPD',
    ['ROLE_ID'] = 'LSFD',
    ['ROLE_ID'] = 'SAHP',
    ['ROLE_ID'] = 'BCSO',
    ['ROLE_ID'] = 'Development',
    ['ROLE_ID'] = 'CIV',
    ['ROLE_ID'] = 'Head Administration'
}

local function getPlayerIdentifierFromType(type, player)
    local count = GetNumPlayerIdentifiers(player)

    for count = 0, count do
        local identifier = GetPlayerIdentifier(player, count)

        if identifier and string.find(identifier, type) then
            if type == 'discord' then
                identifier = string.gsub(identifier, "discord:", "")
            end

            return identifier
        end
    end

    return nil
end

RegisterNetEvent('Framework:Server:getCharacters', function()
    local source = source
    local characters = {}
    local result = MySQL.query.await("SELECT * FROM characters WHERE license = ?", {getPlayerIdentifierFromType('license', source)})

    for i = 1, #result do
        local temp = result[i]
        character = {
            characterId = temp.character_id,
            firstName = temp.first_name,
            lastName = temp.last_name,
            dob = temp.dob,
            gender = temp.gender,
            cash = temp.cash,
            bank = temp.bank,
            department = temp.department
        }

	  characters[#characters + 1] = character
    end

    TriggerClientEvent('Framework:Client:returnCharacters', source, characters)
end)

RegisterNetEvent('Framework:Server:createCharacter', function(data)
    local source = source

    local characterData = json.decode(data)

    local characterId = characterData.CharacterId
    local cash = characterData.Cash
    local bank = characterData.Bank
    local dob = characterData.DoB
    local firstName = characterData.FirstName
    local lastName = characterData.LastName
    local gender = characterData.Gender
    local department = characterData.Department

    local createdCharacter = {
        CharacterId = characterId,
        Cash = cash,
        Bank = bank,
        DoB = dob,
        FirstName = firstName,
        LastName = lastName,
        Gender = gender,
        Department = department
    }

    local result = MySQL.insert.await("INSERT INTO characters (license, first_name, last_name, dob, gender, cash, bank, department) VALUES (?, ?, ?, ?, ?, ?, ?, ?)", {getPlayerIdentifierFromType('license', source), createdCharacter.FirstName, createdCharacter.LastName, createdCharacter.DoB, createdCharacter.Gender, createdCharacter.Cash, createdCharacter.Bank, createdCharacter.Department})

    local characters = {}
    local result2 = MySQL.query.await("SELECT * FROM characters WHERE license = ?", {getPlayerIdentifierFromType('license', source)})

    for i = 1, #result2 do
        local temp = result2[i]
        character = {
            characterId = temp.character_id,
            firstName = temp.first_name,
            lastName = temp.last_name,
            dob = temp.dob,
            gender = temp.gender,
            cash = temp.cash,
            bank = temp.bank,
            department = temp.department
        }

	  characters[#characters + 1] = character
    end

    TriggerClientEvent('Framework:Client:returnCharacters', source, characters)
end)

RegisterNetEvent('Framework:Server:deleteCharacter', function(characterId)
    local source = source

    local result = MySQL.query.await('DELETE FROM characters WHERE character_id = ? LIMIT 1', {characterId})

    local characters = {}
    local result2 = MySQL.query.await("SELECT * FROM characters WHERE license = ?", {getPlayerIdentifierFromType('license', source)})

    for i = 1, #result2 do
        local temp = result2[i]
        character = {
            characterId = temp.character_id,
            firstName = temp.first_name,
            lastName = temp.last_name,
            dob = temp.dob,
            gender = temp.gender,
            cash = temp.cash,
            bank = temp.bank,
            department = temp.department
        }

	  characters[#characters + 1] = character
    end

    TriggerClientEvent('Framework:Client:returnCharacters', source, characters)
end)


RegisterNetEvent('Framework:Server:editCharacter', function(data)
    local source = source

    local characterData = json.decode(data)

    local characterId = characterData.CharacterId
    local cash = characterData.Cash
    local bank = characterData.Bank
    local dob = characterData.DoB
    local firstName = characterData.FirstName
    local lastName = characterData.LastName
    local gender = characterData.Gender
    local department = characterData.Department

    local createdCharacter = {
        CharacterId = characterId,
        Cash = cash,
        Bank = bank,
        DoB = dob,
        FirstName = firstName,
        LastName = lastName,
        Gender = gender,
        Department = department
    }

    local result = MySQL.query.await("UPDATE characters SET first_name = ?, last_name = ?, dob = ?, gender = ?, department = ? WHERE character_id = ? LIMIT 1", {createdCharacter.FirstName, createdCharacter.LastName, createdCharacter.DoB, createdCharacter.Gender, createdCharacter.Department, characterId})

    local characters = {}
    local result2 = MySQL.query.await("SELECT * FROM characters WHERE license = ?", {getPlayerIdentifierFromType('license', source)})

    for i = 1, #result2 do
        local temp = result2[i]
        character = {
            characterId = temp.character_id,
            firstName = temp.first_name,
            lastName = temp.last_name,
            dob = temp.dob,
            gender = temp.gender,
            cash = temp.cash,
            bank = temp.bank,
            department = temp.department
        }

	  characters[#characters + 1] = character
    end

    TriggerClientEvent('Framework:Client:returnCharacters', source, characters)
end)

local function discordRequest(method, endpoint, jsondata)
    local data
    local headers = {
        ['Content-Type'] = 'application/json',
        ['Authorization'] = 'Bot BOT_TOKEN_HERE'
    }

    PerformHttpRequest(('https://discord.com/api/%s'):format(endpoint), function(errorCode, resultData, resultHeaders)
        data = { data = resultData, code = errorCode, headers = resultHeaders }
    end, method, #jsondata > 0 and json.encode(jsondata) or "", headers)

    repeat Wait(0) until data

    return data
end

RegisterNetEvent('Framework:Server:getDiscordRoles', function()
    local source = source
    local discordId = getPlayerIdentifierFromType('discord', source)

    if not discordId then
        return
    end

    local endpoint = string.format('guilds/%s/members/%s', 'GUILD_ID_HERE', discordId)
    local member = discordRequest('GET', endpoint, {})
    Wait(100)

    if member.code == 200 then
        local data = json.decode(member.data)
        local roles = data.roles

        local rolesFound = {}

        for roleId, roleName in pairs(roleIds) do
            for _, role in ipairs(roles) do
                if tostring(role) == roleId then
                    rolesFound[roleId] = roleName
                    break
                end
            end
        end

        if next(rolesFound) then
            for roleId, roleName in pairs(rolesFound) do
                print(('%s has role: %s (%s)'):format(GetPlayerName(source), roleName, roleId))
            end

            TriggerClientEvent('Framework:Client:returnDiscordRoles', source, json.encode(rolesFound))
        else
            print('%s has no matching discord roles'):format(GetPlayerName(source))
        end
    else
        print('failed to retrieve member data: %s'):format(member.code)
    end
end)

Citizen.CreateThreadNow(function()
    local guild = discordRequest('GET', 'guilds/GUILD_ID_HERE', {})

    if guild.code == 200 then
        local data = json.decode(guild.data)
        print(('Successfully connected to guild %s (%s)'):format(data.name, data.id))
    else
        print(('Failed to connect to guild: %s'):format(guild.data or guild.code))
    end
end)