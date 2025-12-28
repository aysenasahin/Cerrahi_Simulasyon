# VR-Supported Robotic Surgery Simulation (Capstone Project) 🏥🤖

This project is a **Digital Twin** framework and simulation environment for a 3D-printed robotic surgery system, developed using **Unity 3D**.

Designed as a senior engineering **Capstone Project**, it aims to create an immersive **Virtual Reality (VR)** experience for medical training and research, where users can interact with surgical instruments in a physics-based environment.

## 🚀 Project Overview

The primary goal of this project is to bridge the gap between a physical surgical robot and a virtual simulation. While the team focuses on the hardware manufacturing of the robot, this repository hosts the software infrastructure that will eventually synchronize with the physical unit via **ROS2** and **Wi-Fi** protocols.

### ✅ Current Features
* **Virtual Operating Room:** A photorealistic, high-fidelity operating room environment.
* **Physics-Based Interaction:** Mechanics allowing users to grab, hold, and manipulate surgical tools (scalpels, scissors, etc.) realistically within the VR space.
* **Unity 6 Integration:** Built on the latest Unity engine for advanced rendering and physics.

### 🔮 Roadmap & Future Goals
The following features are planned for the next phase of development:
* **ROS2 Bridge:** Implementing a communication bridge to transmit joystick data to the simulation.
* **Real-Time Synchronization:** Syncing the movements of the physical 3D-printed robot with this Digital Twin.
* **Image Processing:** Integrating computer vision to enhance the feedback loop between the real and virtual environments.

## 🛠️ Tech Stack
* **Engine:** Unity 6 (6000.x)
* **Language:** C#
* **Platform:** PC / VR
* **Protocols (Planned):** ROS2, Wi-Fi, UDP/TCP

## 🎨 Assets & Credits

Special thanks to **Digital Surgery** for providing high-quality 3D models of surgical instruments used in this simulation.

* **3D Models:** [Digital Surgery on Sketchfab](https://sketchfab.com/digitalsurgery)
* **Environment:** Charité - Universitätsmedizin Berlin (Reference/Assets)

## 📥 Installation & Setup

To run this project locally:

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/aysenasahin/Cerrahi_Simulsyon]
    ```

2.  **Open in Unity Hub:**
    * Click `Add` and select the cloned folder.
    * Ensure you have **Unity 6 (6000.0.28f1 or later)** installed.

3.  **Run the Scene:**
    * Navigate to `Assets/Scenes`.
    * Open `SampleScene`.
    * Press **Play**.

---

### 👩‍💻 Author
**Ayşe Sena Şahin** - *Electrical-Electronics Engineering Student*
*Başkent University*
