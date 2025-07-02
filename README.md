# 🏫 LLM-Classroom: AI-Powered Virtual Classroom 🎬

Welcome to **LLM-Classroom**! This innovative project combines local large language models with Unity-rendered virtual environments to create engaging educational experiences featuring animated characters in a dynamic classroom setting.

## ✨ What It Does

**Backend (Python)**: Uses local LLMs (Ollama) to generate synthetic classroom discussions, educational episodes, and interactive content. Features a REST API for seamless content distribution and live chat integration.

**Frontend (Unity)**: A beautiful 3D virtual classroom environment where characters like Richard Feynman, Alan Watts, and student Alice engage in discussions, with dynamic blackboards displaying generated content and web-scraped images.

## 🎥 See It In Action

[![Watch a classroom episode on YouTube](https://img.youtube.com/vi/0QkFQwB1p6A/0.jpg)](https://www.youtube.com/@SteffenProbst-qt5wq/streams)  
👉 [Experience the project live on YouTube!](https://www.youtube.com/@SteffenProbst-qt5wq/streams)

## 🚀 Quick Start

### Prerequisites
- Python 3.11+
- Unity 2022.3+ (URP)
- Ollama (for local LLMs)

### Backend Setup
```bash
cd backend\ \(python\)/
pip install -r episode_generator_requirements.txt
pip install -r rest_api_requirements.txt
# Start Ollama: ollama serve
# Pull models: ollama pull llama2 && ollama pull orca2
```

### Frontend Setup
```bash
cd frontend\ \(unity\)/
# Open in Unity Editor
# Press Play to explore the classroom
```

## 🏗️ Architecture

- **Content Generation**: Python scripts create educational episodes using LLMs
- **REST API**: Distributes content to Unity environment
- **3D Rendering**: Unity displays characters, animations, and dynamic content
- **Live Integration**: Web scraping and chat processing for audience interaction

## 📁 Project Structure

```
LLM-Classroom/
├── backend (python)/          # Python backend system
│   ├── classes/              # Core data structures
│   ├── interface/            # LLM and API interfaces  
│   ├── scripts/              # Episode generation & REST API
│   └── few_shot_examples/    # LLM training examples
└── frontend (unity)/         # Unity frontend system
    ├── Assets/               # Unity assets & scenes
    ├── ProjectSettings/      # Unity configuration
    └── Packages/             # Unity dependencies
```

## 🤝 Contributing

I welcome all forms of engagement! From GitHub stars to feedback and pull requests - every interaction helps drive this project forward.

## 💰 Support

This project incurs costs for hosting and compute. If you find it useful, please consider supporting the development.

## 📞 Contact

For collaboration, questions, or feedback: [GitHub Profile](https://github.com/Probst1nator)

## 📜 License

GNU General Public License - see LICENSE file for details. 