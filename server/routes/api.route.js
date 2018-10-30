module.exports = (app) => {
  const api = require('../controllers/api.controller.js');
  
  //Display all commands
  app.get('/', api.getAllCommands);

  //Display all bots
  //JSON Array mit bots (und deren info)
  app.post('/bot', api.addTask);
}