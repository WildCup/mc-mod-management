//init
// var _input = @"C:\Users\hagel\AppData\Roaming\.minecraft\mods";
var input = @"C:\Users\hagel\AppData\Roaming\.minecraft\mods";
var files = new FileLogic(input);

//main logic
var main = new Logic(files.Mods, files.ModsKnown);
main.Sync(files.ModsInput);

//save
FileLogic.Save(main.Mods, main.ModsKnown);

Console.WriteLine("File names saved to JSON successfully");

// var test = new FileLogicIntegrationTest();
// test.WhenUpdatedDependency();
