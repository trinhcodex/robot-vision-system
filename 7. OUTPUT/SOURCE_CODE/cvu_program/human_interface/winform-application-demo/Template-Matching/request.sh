curl -X POST -H "Content-Type: multipart/form-data" \
			 -F "api_folder=/home/kratosth/code/robot-vision-system/7. OUTPUT/SOURCE_CODE/cvu_program/human_interface/winform-application-demo/Template-Matching" \
			 -F "output_folder=Output" \
			 -F "img_path=Dataset/Src2.bmp" \
			 -F "template_path=Dataset/20220611.bmp" \
			 -F "threshold=0.75" \
			 -F "overlap=0.4" \
			 -F "method=cv2.TM_CCOEFF_NORMED" \
			 -F "min_modify=-1" \
			 -F "max_modify=1" \
			 -F "enhance=Custom_enhance/Src1-2.json" \
			 -F "representation=Custom_representation/Src3-5-8-9-10.json" \
			 http://127.0.0.1:5000/my_cvu_api