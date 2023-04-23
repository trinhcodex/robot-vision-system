# Note

Unzip Winform-demo.zip and copy the folder `.u2net` to home folder `C:\Users\[Name]`

# Usage

Click to `WinFormCVU.exe` to open the application

Appearance:
![alt text](https://cdn.discordapp.com/attachments/1081256318846894242/1099674576788656181/image.png)

* `Score` param: The similarity score between template and objects in the target image `Using 0.75 as default value`

* `Overlap Ratio` param: Using to remove redundent bounding box `Using 0.4 as default value`

* `Range` param: The range to modify the proposal angles `Using -1 and 1 as default value`

* `Method` param: The method to get similarity score between template and RoI in the target image

* `IP` param: The IP of camera `Example: 192.168.1.9`

* `Connect` button: To get a image from camera via IP address

* `Match` button: To perform pattern matching algorithm

* `Template` and `Target Image`: Drag and drop the image from `Dataset` folder. In addition, you can create the new `Template` by using mouse to cut the RoI in `Target Image` and click the `Add Template` button

* `TextBoxes` in `Scale`: The first `TextBox` is the value to zoom in or zoom out the `Target Image`. The second `TextBox` is the percentage of size of `Target Image` with original image. You have to zoom in or zoom out the `Target Image` to get 100% percentage before cutting `Template` via mouse

* `Up` and `Down` button: Using to resize the `Target Image`. You can use 2 `ScrollBar` to move the `Target Image` to the position you want

* `Enhance browse` button: Getting a file json from `Custom_enhance` folder

* `Representation browse` button: Getting a file json from `Custom_representation` folder
