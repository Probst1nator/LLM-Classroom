# 🎮 Frontend: Unity AI Classroom

The Unity frontend that renders the 3D virtual classroom environment with animated characters and dynamic content display.

## 🚀 Quick Setup

1. **Open in Unity**: Open the `frontend (unity)` folder in Unity 2022.3+ with URP support
2. **Load Scene**: Open `Assets/classroom_location.unity`
3. **Press Play**: Explore the virtual classroom environment

## 🎭 Key Features

- **Interactive 3D Classroom**: Beautiful Japanese school environment
- **Animated Characters**: Richard Feynman, Alan Watts, and student Alice
- **Dynamic Blackboard**: Displays generated content and web-scraped images
- **REST API Integration**: Connects to Python backend via `RestClient.cs`

## 📁 Important Files

- **`Assets/RestClient.cs`**: Main API integration script
- **`Assets/classroom_location.unity`**: Primary classroom scene
- **`Assets/Characters/`**: Character models and animations
- **`Assets/JapaneseSchool/`**: Classroom environment assets
- **`Assets/StreamingAssets/`**: Backend-generated content (linked)

## 🔧 Configuration

- **API Connection**: Configure backend URL in `RestClient.cs`
- **Character Setup**: Add new characters to the scene hierarchy
- **Scene Detection**: Update `FindSupportedScenes()` for new interactable objects

## 🎬 Livestream Output

The Unity project is configured for real-time rendering suitable for YouTube streaming with optimized performance settings.

For detailed documentation, see the main project README.
