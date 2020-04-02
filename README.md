# OpenFace-FACS-Unity-Facial-Animator
## A Unity facial animation tool using the Facial Action Coding System (FACS)

### About

This is a mostly-easy-to-use, mostly-standalone facial animation tool for Unity. Allowing the user to stream live OpenFace data to Unity as FACS (Facial Action Coding System) values, anyone can simply hook this simple tool up to their facial rig inside of Unity and animate facial blendshapes with it. The [Facial Action Coding System (FACS)](https://en.wikipedia.org/wiki/Facial_Action_Coding_System "https://en.wikipedia.org/wiki/Facial_Action_Coding_System") by Paul Ekman describes facial expressions in terms of muscle groups, called Action Units (AUs). By sending these AUs from OpenFace to Unity a value between 0-1, we can translate this contraction & relaxation of facial muscles as blendshape-friendly data!

This repository uses a modified version of the FACSvatar framework built by [Stef van der Struijk, Hung-Hsuan Huang, Maryam Sadat Mirzaei and Toyoaki Nishida](https://github.com/NumesSanguis/FACSvatar) and uses [ZeroMQ ](http://zeromq.org/) to stream data from OpenFace to Unity.

### How to Install

1. Clone or download this repository.

2. Go to the [release page of this GitHub repo](https://github.com/NumesSanguis/FACSvatar/releases) and download *openface_2.1.0_zeromq.zip*.
    * Unzip and execute `download_models.sh` or `download_models.ps1` to download trained models

3. Install Docker. It lets you run applications without worrying about OS or programming language and is widely used in machine learning contexts.
    * [Windows 7/8/10 Home](https://docs.docker.com/toolbox/overview/)
    * [Windows 10  Pro, Enterprise or Education](https://docs.docker.com/docker-for-windows/install/#what-to-know-before-you-install)
    * Linux (untested): [Docker](https://docs.docker.com/install/linux/docker-ce/ubuntu/) and [docker-compose](https://docs.docker.com/compose/install/) and `sudo usermod -a -G docker $USER`
    
4. Open the Unity project, navigate to the Asset Store and install **JSON .NET for Unity (by PARENTELEMENT, LLC)**.

5. **If you're using Windows Home (7/8/10)**
    * Navigate inside the OpenFace folder you downloaded
    * (Windows 7/8/10 Home - only) Get your Docker machine ip by opening a 2nd terminal and running: `docker-machine ip` (likely to be 192.168.99.100)
    * (Windows 7/8/10 Home - only) Open `config.xml`, change `<IP>127.0.0.1</IP>` to `<IP>machine ip from step 3</IP>` (`<IP>192.168.99.100</IP>`), save and close.



### How to Use

1. Launch Docker. If it asks you (or if you ask yourself), use Linux containers.

2. It's time to launch the instance of ZeroMQ that will stream OpenFace data to Unity. With Docker installed and the models downloaded, open a terminal (W7/8: cmd.exe / W10: PowerShell) and navigate to the *modules* folder, then execute:
    1. `docker-compose pull`  (Downloads FACSvatar Docker containers)
    2. `docker-compose up`  (Starts downloaded Docker containers)

3. Open the *OpenFace* folder and run *OpenFaceOffline.exe*. Inside OpenFace, select *File -> Open Webcam*.If everything works properly, you should see Action Units bounce around in OpenFace and data scroll inside of the powershell window.

4. Launch Unity. If everything works properly, you should be able to open the demo scene, press play and see the cartoon face come to life!



