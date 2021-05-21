import cv2

fps = 18

images = []

frameindex = 0
lastimg = None
while True:
    img = cv2.imread(f'out/video{frameindex}.png')

    if img is None:
        break

    lastimg = img

    images += [img]

    frameindex += 1

# The last frame lasts longer
images += [lastimg] * fps

height, width, layers = images[0].shape

video = cv2.VideoWriter('out/split.avi', 0, fps, (width, height))

for img in images:
    video.write(img)

cv2.destroyAllWindows()
video.release()

