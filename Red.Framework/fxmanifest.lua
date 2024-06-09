fx_version 'cerulean'
game 'gta5'

author 'RedMeansWar'
description 'FiveM Framework script written in C#'
version '1.0.0'

ui_page 'html/index.html'

dependency 'oxmysql'

files {
    'Red.Common.Client.net.dll',
    'Newtonsoft.Json.dll',
    'html/index.html',
    'html/style.css',
    'html/script.js',
    'html/imgs/*.png'
}

client_script 'Red.Framework.Client.net.dll'
server_scripts {
    '@oxmysql/lib/MySQL.lua',
    'FrameworkHelper.lua'
}
