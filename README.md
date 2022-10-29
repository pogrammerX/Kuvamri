# Kuvamri
A NodeJS replacement.

# How to use
This is easy, just a little code here and there, and that'll do it!

These are all the files you'll need

MyProject\app.config
```config
{
  "err500": "error500.html",
  "err404": "error500.html",
  "err403": "error500.html"
}
```
This file is pretty self explaining.
  
MyProject\index.kri
```kri
server.listen("/", (stream)->{stream.html=<h1>hi!</h1>})
```
Here we are just listening for the / get command, and sending <h1>hi!</h1>

Thats it!

(Currently none of the defined filenames are used, but they are still needed to be defined.)
