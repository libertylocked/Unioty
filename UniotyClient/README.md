# Unioty Client

## Usage
- `unioty.py` is the Unioty client library

## Demo 1

### Requirements
- Hardware Requirements
  - Requires Raspberry Pi, GrovePi, Grove Button, Grove HDC1000
- Software Requirements
  - Copy [grovepi.py](https://github.com/DexterInd/GrovePi/blob/master/Software/Python/grovepi.py) and [grove_i2c_temp_hum_hdc1000.py](https://github.com/DexterInd/GrovePi/blob/master/Software/Python/grove_i2c_temp_hum_hdc1000/grove_i2c_temp_hum_hdc1000.py) from Dexter Industries [GrovePi](https://github.com/DexterInd/GrovePi) repository to this folder

### Running the demo
- Connect Grove button to `D3`, HDC1000 to any I2C port
- Change the server address hardcoded in Python scripts
  - Edit `hub_demo1_buttons.py` and change the server address
  - Edit `hub_demo1_hdc1000.py` and change the server address
- Run the demo
  - Run `demo1` scene in Unity
  - Run `hub_demo1_buttons.py` and `hub_demo1_hdc1000.py` on Raspberry Pi
- Play with the demo!
  - Pressing the button will make the ball jump
  - The temperature data is displayed on a 3D text in game


## Demo 2 (VR)
Demo 2 is built for VR. It synchronizes the color of a real RGB LED connected to a Raspberry Pi with the RGB light in-game.

- Hardware Requirements
  - Requires Raspberry Pi, GrovePi, Grove Chainable RGB LED
  - Requires a SteamVR compatible headset (e.g. HTC Vive, OSVR) on the host
- Software Requirements
  - Copy [grovepi.py](https://github.com/DexterInd/GrovePi/blob/master/Software/Python/grovepi.py) from Dexter Industries [GrovePi](https://github.com/DexterInd/GrovePi) repository to this folder

### Running the demo
- Connect Grove Chainable RGB LED to `D7`
- Change the server address hardcoded in Python scripts
  - Edit `hub_demo2_rgb.py` and change the server address
- Run the demo
  - Run `demo2vr` scene in Unity
  - Run `hub_demo2_rgb.py` on Raspberry Pi
- Play with the demo!
  - The 3 buttons above the number displays increase the R, G, B value of the light respectively
  - The 3 buttons below the number displays decrease the R, G, B value of the light respectively
  - Whenever the color of the light is changed, the color of the RGB LED on Pi is also updated
