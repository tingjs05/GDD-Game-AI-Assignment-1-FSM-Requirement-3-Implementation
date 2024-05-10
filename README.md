# Description
This is the implementation of the Assassin-Bot AI for the Game AI (GMAI) Assignment 1 using Finite State Machines. 

# Details
In the middle of the night, you heard a sound. An intruder!  You quickly grab your lantern, and run out of your bedroom. You see an assassin-robot patrolling the hallway with the words PATROL written on the LED screen on its head. It must be sent by your jealous business rival to eliminate you to take your company out of the competition. 
<br><br>
Suddenly, the words on its head changed to ALERT as it slowly moved in your direction, you must have gotten too close to the bot. You try to back away, but the words on its head suddenly turn into PROWL, it must have seen you. It then quickly makes a beeline in your direction. You heard something, turned around, and moved towards it. “Is someone there?” you shouted, but there was no one. The bot must be in its HIDE state. “There must be nothing there…” you thought as you turned away and continued searching for the bot. As you turn around, the words on the bot turn red, and displays ATTACK. It swings in your direction, but its attack somehow misses. The words then display FLEE as the bot retreats back into the shadows. 
<br><br>
As it roams the hallways, it finds corridors where it can lay traps. Once it has found the perfect corridor, the words on its head display LAY TRAP as it bends down and installs a trap on the floor, and continues roaming the house, looking for its prey. 
<br><br>
Since it is very dark, it is hard for you to see. Before you know it, you have accidentally triggered a trap. Ropes are deployed and you are held in place. You try to wiggle out, but you could not get out in time. The words on the head of the robot turn red and display TRAP TRIGGERED as it rushes towards the location of the trap. Once it sees you the words quickly flash PROWL and ATTACK. Luckily, you managed to wiggle out in time, and managed to escape this close encounter. The bot then returns to its PATROL state. 
<br><br>
In the house, there are also many display cases and shelves that are not too stable. The bot notices this and decides to lay an ambush. It finds a shelf nearby, it then hides there and waits for the player to walk by, displaying the words WAIT on its head. When the player walks by, the word on its head changes to PUSH as the bot comes out of its hiding spot, walks towards the shelf and pushes it, hoping to trap you. 
<br><br>
After you narrowly escaped from that, you tried to do the same thing to the bot. You wait, and finally have the opportunity. You push another shelf, and it falls on the bot. The bot appears to be stunned, displaying the word STUNNED on its head, and looks slightly damaged. You take this time to try getting out of the house. However, after a while, the words on the bot’s head returned to PATROL. It notices you trying to get away and rush at you. 
<br><br>
Noticing the damage on the bot after dropping the shelf on it, you repeated the process of dropping stuff on the bot and stunning it. Eventually, the words on his head display DEATH as it falls to the ground and lay motionless, allowing you to escape. 

# FSM Overview
![FSM Diagram](https://github.com/tingjs05/GDD-Game-AI-Assignment-1-FSM-Requirement-3-Implementation/assets/105273734/38e9da46-66e2-4e7c-9fc0-cf8a62cb96c2)

