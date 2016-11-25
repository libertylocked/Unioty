# UnioTy
Connect IoT devices to your Unity game!

## Features
- Allows IoT devices to send data to your game
- Both TCP and UDP are supported!
  - TCP for triggers (e.g. button presses and releases)
  - UDP for analog data (e.g. temperature readings)
- Event-based callbacks: GameObjects who subscribe to a data source will be notified when data is ready

## Use cases
- Real time sensor data visualization
- Game peripherals

## Running demo 1
- Hardware setup
  - Requires Raspberry Pi, GrovePi, Grove Button, Grove Magnetic Switch, Grove HDC1000
  - Connect Grove button to `D3`, magnetic switch to `D2`, HDC1000 to any I2C port
- Change the server address hardcoded in Python scripts
  - Edit `UniotyPi/hub_demo1_buttons.py` and change the server address
  - Edit `UniotyPi/hub_demo1_hdc1000.py` and change the server address
- Run the demo
  - Run the demo1 scene in Unity
  - Run `hub_demo1_buttons.py` and `hub_demo1_hdc1000.py` on Raspberry Pi
- Play with the demo!
  - Pressing the button will make the ball jump
  - Putting a magnet on the magnetic switch will make the ball larger
  - The temperature data is displayed on a 3D text in game
  
