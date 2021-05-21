# Source https://gist.github.com/michaelosthege/cd3e0c3c556b70a79deba6855deb2cc8

import imageio
import os, sys

class TargetFormat(object):
    GIF = ".gif"
    MP4 = ".mp4"
    AVI = ".avi"

def convertFile(inputpath, targetFormat):
    """Reference: http://imageio.readthedocs.io/en/latest/examples.html#convert-a-movie"""
    outputpath = os.path.splitext(inputpath)[0] + targetFormat
    print("converting\n\t{0}\nto\n\t{1}".format(inputpath, outputpath))

    reader = imageio.get_reader(inputpath)
    fps = reader.get_meta_data()['fps']

    writer = imageio.get_writer(outputpath, fps=fps)
    for i,im in enumerate(reader):
        sys.stdout.write("frame {0}\n".format(i))
        sys.stdout.flush()
        writer.append_data(im)
    print("\nFinalizing...")
    writer.close()
    print("Done.")

convertFile("out/split.avi", TargetFormat.GIF)
