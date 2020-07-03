# antykutasator
## Face Detection and Recognition Using OpenCV
Let’s talk about a situation when you or one of your coworkers walks away from the computer without locking the screen. Amazement after coming back can be enormous, especially, if creativity of our coworkers is above the average.

I usually remember to lock the screen but I don’t want to take the risk, and I wrote a simple application which works in the background. It downloads the image from the laptop’s webcam from time to time, and it tries to detect user’s face.
The script downloads and analyses the image every few seconds. If the program won’t detect user’s face few times in a row, it automatically locks the screen
