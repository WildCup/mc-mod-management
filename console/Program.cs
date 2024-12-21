using McHelper;
using McHelper.Application.Logic;

var file = new FileLogic();
var logic = new Logic(file.Mods, file.ModsKnown);

logic.Sync(file.ModsInput);
file.Save(logic.Mods, logic.ModsKnown);

var ftp = new FtpLogic();
ftp.Sync(file.ModsInput, logic.Mods, true);
// ftp.CompareConfig();
