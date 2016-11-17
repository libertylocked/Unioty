# UnioTy
Let IoT devices power your Unity game!

## What UnioTy platform does
- It lets IoT devices to send data to your game
- It allows you to control your GameObjects based on input

## Use cases
- Real time sensor data visualization
- Game peripherals

## Running the demo
- Requires Raspberry Pi + GrovePi + Grove Button
- Connect the grove button to digital port `D3`
- Edit `ControllerHub/hub_button.py` and change the server address
- Build and run server on localhost
- Run the demo game on localhost
- Run `hub_button.py` script on Raspberry Pi
- Press the button should make the ball fly upwards
