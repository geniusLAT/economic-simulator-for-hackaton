using economic_simulator_for_hackaton;

var webService = new WebService();
var task = Task.Run(() => webService.ExecuteAsync([]));

task.Wait();
