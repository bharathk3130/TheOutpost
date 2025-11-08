# The Outpost
A third-person shooter where the player must reach and transmit a signal while surviving against waves of AI enemies trained with reinforcement learning. The agent learns to patrol, aim, and attack intelligently using Unity ML-Agents, adapting its behavior through rewards. The player’s goal is to stay alive long enough to send the transmission while the enemies relentlessly try to stop them.

The agents were trained across 3 stages using Curriculum Learning. The stages are:
- Stage 1: Shooting
- Stage 2: Shooting + Aiming
- Stage 3: Shooting + Aiming + Movement

---

## 🎮 How to Play
1. Navigate to `Builds/TheOutpost/TheOutpost.exe`
2. Launch the game.
3. Survive against the AI enemies and reach the signal point.
4. Transmit the signal before you’re eliminated or the battery runs out. The battery fills up while you’re in the transmission zone and depletes when you’re outside of it.
5. You can stand in the healing pod outside the transmission zone to restore health.
6. **WASD** to move, **Shift** to sprint, **left click** to shoot, **right click** to aim, **space** to jump, **R** to reload and **Escape** to unlock cursor

---

## 🛠️ Setting Up the Project
1. Create a **Unity 3D URP Project** using **Unity 6000.0.31f1**  
2. Delete the default `Assets` folder.  
3. Replace it with the `Assets` folder from this repository.  
4. Copy the `Builds`, `config`, and `results` folders from this repo into your project directory.  

---

## 🤖 Training the Agents

### 1. Install ML-Agents and Python
Follow the official Unity ML-Agents setup guide:  
👉 [Unity ML-Agents Installation Guide](https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md)

Once Python and ML-Agents are installed, open the project in Unity, open **Unity Package Manager**  and download **ML-Agents**

### 2. Start training
- Open a **Miniforge Prompt** and navigate to your project folder  
- Run:
  ```bash
  conda activate mlagents
  mlagents-learn config/stage3_move.yaml --run-id=test_stage3/test_stage3_01 --env=Builds/Training/Stage1_Shooting/FortniteML2.exe --initialize-from=stage3/stage3_02 --no-graphics --num-envs=8

This command:
- Runs 8 parallel training instances with no graphics
- Initializes weights from Stage 3’s trained model
- Uses stage 3's configuration to train it

Modify the flags as per your requirement
