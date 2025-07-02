# 🐍 Backend: Livestream Script Generator

The Python backend that powers the LLM-Classroom system, generating educational content using local large language models.

## 🚀 Quick Setup

```bash
# Install dependencies
pip install -r episode_generator_requirements.txt
pip install -r rest_api_requirements.txt

# Start Ollama
ollama serve

# Pull required models
ollama pull llama2
ollama pull orca2
```

## 📁 Key Components

- **`classes/`**: Core data structures (Episode, Action, etc.)
- **`interface/`**: LLM clients and API interfaces
- **`scripts/`**: Main execution scripts
  - `generateEpisodes.py`: Creates educational content
  - `restApi.py`: Serves content to Unity frontend
  - `chatProcessor.py`: Handles live chat integration
- **`few_shot_examples/`**: LLM training examples

## 🔧 Usage

```bash
# Start REST API server
python scripts/restApi.py

# Generate episodes
python scripts/generateEpisodes.py
```

## 📡 API Endpoints

- `GET /chooseEpisodePath` - Get next episode
- `GET /getEpisode?path=<path>` - Retrieve episode data
- `GET /getAudio?episodePath=<path>&character=<name>&actionIndex=<index>` - Get audio
- `GET /getPoll` - Get audience poll data

For detailed documentation, see the main project README.
