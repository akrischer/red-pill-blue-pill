# Terminus

A tiny little narrative story told squarely through a computer terminal.

### Context

Terminus was developed as a project for a class where we studied Apocalypticism in Film. Personally, I was interested in developing my narrative design skills.
I wanted to experience what it's like to develop a naiive little story, told squarely through text interactions in a computer terminal.


One of my goals too was having a hand at implementing a dialog tree. The dialog tree implementation I created uses a Broadcaster/Subscriber Node model
which has broadcasters broadcast messages to subscribing message nodes who in turn point to more broadcaster nodes. Currently, the model is slightly coupled with
the game itself, so I plan to implement the following improvements (following this [wonderful blog entry](http://etodd.io/2014/05/16/the-poor-mans-dialogue-tree/)):


  - **Set Node**: Sets a variable, in the scope of the dialog tree, to any arbitrary **value**.
  - **Branch Node**: Takes one of several paths based on the value of a variable.
  - **Event Node**: Can call an arbitrary method on a game object with arbitrary arguments
  - **Empty Node**: Does nothing, but can link to any other node


The dialog tree builds nodes from input JSON, easing the process of creating a UI, as the UI must only output JSON.
