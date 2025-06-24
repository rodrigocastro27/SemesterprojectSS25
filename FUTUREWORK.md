The semester project started with an ambitious idea and not everything was implemented. In addition, due to the time limitations, some parts of the code can be cleaned, refactored, or improved, and some inconsistencies could be avoided. 

The list of future work we propose is the following.

Additionally, throughout the documentation, there are tips and improvements specified with:

❗<mark>TODO: *description*</mark> .

---
# **Improvements**

## Error Management
- [ ] Set up a proper error management system both in the **server** and the **Flutter app**. Mostly because there are a lot of potential unhandled Null Pointer exceptions (where in the code they are simply ignored by using `!` and `?` in the variables definitions and retrievals).
## Server Deployment
- [ ] Find a proper way to **deploy** and connect to the server without using `ngrok`.
- [ ] Deploy the server in a **docker** container.
- [ ] Generate a simple UI or **console application** for the server to see the logs.
## Database Improvement
- [ ] Create a function for creating/resetting the database. It does not exist because the database `database.db` was created manually with an interface instead of with queries.
## Implement Authentication
- [ ] Right now, the players are distinguished by their unique username. A good authentication system that lets them log in with an email and a unique id should be implemented.
## UI
- [ ] Finish all "new" UI ([more info](/docs/api/switchUI).
- [ ] Implemented in the final version of the game.

---
# **Features**
## Game Modes
Initially, we defined three *game modes* that could be implemented and chosen before starting the game:
#### **Classic Hide & Seek**
Online version of the traditional game. Only includes the time and zone limitations, and the ping to see the hiders. In addition, tasks and abilities.
#### **Battle Royale Mode**
It has all the features in the *Classic Hide & Seek* but additionally includes a battle-royale-style zone that shrinks and defines where the players can play. It can also define where tasks can spawn (in the case that tasks appear randomly in the map and the players have to physically move to start them).
#### **Custom Game Mode**
A customizable mode where the players in the lobby are able to personalize the features of the game (e.g., selecting time to play, randomize tasks or not). Additional feature: select the possibility of role changing (mid-game seekers can become hiders and vice versa). 
### Others
These are some general improvements/features still to be implemented:
- [ ] Right now, the game does not have any **starting conditions** that need to be implemented. For instance, have a minimum number of hiders and seekers, or a minimum number of player per lobby.
- [ ] Once a player is **eliminated**, it should be able to see the game continue without being redirected to the game over screen. And the rest of players could have a way to see who is eliminated in the lobby.
- [ ] Implement a **notifications system** so all players are constantly notified of changes in the game (e.g., when a player is eliminated).
## Tasks
We had originally planned some tasks to develop for nice teamwork dynamics that have not been implemented:
- [ ] Have specific tasks that can be played only by hiders or seekers and they can either win (and gain an ability) or loose. The idea of these tasks would be to appear on the map and the first player (hider or seeker) to reach it gains the possibility of playing for its team.
- [ ] Have the possibility of easily creating and customizing tasks read from a QR code put in the playing zone.
- [ ] **Task:** create a task where a spot in the map appears and the first player to go there wins.
- [ ] **Task:** a *wordle* style task.
- [ ] **Task:** a task where two players of the same team need to be at a specific distance and complete a task in teamwork.
- [ ] **Task:** multiple-choice styled task about information of the players, the university, the companies in Sofwarepark as a get-to-know task.
## Abilities
Similarly to the tasks, we also had some ideas on new abilities the players could win after completing tasks:
### Hider Abilities
- [ ] Decrease searching time / Increasing playing area
- [ ] Create a fake ping
- [ ] Include multiple ping locations of yourself in the map
### Seeker Abilities
- [ ] Increase searching time / Decrease playing area
- [ ] See hider's footprints a few seconds after ping

---
## Good luck!!
