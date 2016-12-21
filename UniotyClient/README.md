# Unioty IoT Platform Client

## Usage
- `unioty.py` is the Unioty client library

## Demos

### Demo 1

#### Requirements
- Hardware Requirements
  - Requires Raspberry Pi, GrovePi, Grove Button, Grove HDC1000
- Software Requirements
  - Copy [grovepi.py](https://github.com/DexterInd/GrovePi/blob/master/Software/Python/grovepi.py) and [grove_i2c_temp_hum_hdc1000.py](https://github.com/DexterInd/GrovePi/blob/master/Software/Python/grove_i2c_temp_hum_hdc1000/grove_i2c_temp_hum_hdc1000.py) from Dexter Industries [GrovePi](https://github.com/DexterInd/GrovePi) repository to this folder

#### Running the demo
- Connect Grove button to `D3`, HDC1000 to any I2C port
- Change the server address hardcoded in Python scripts
  - Edit `hub_demo1_buttons.py` and change the server address
  - Edit `hub_demo1_hdc1000.py` and change the server address
- Run the demo
  - Run the `demo1` scene in Unity
  - Run `hub_demo1_buttons.py` and `hub_demo1_hdc1000.py` on Raspberry Pi
- Play with the demo!
  - Pressing the button will make the ball jump
  - The temperature data is displayed on a 3D text in game
