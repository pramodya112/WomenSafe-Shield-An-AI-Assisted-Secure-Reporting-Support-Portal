# 🛡️ SafeReport — Women Empowerment Portal

> A safe, powerful digital space built by women, for women. Your voice matters. Your safety matters. **You matter.**

![Angular](https://img.shields.io/badge/Angular-18+-DD0031?style=flat-square&logo=angular&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Groq](https://img.shields.io/badge/AI-Groq%20%2F%20Llama%203.1-orange?style=flat-square)
![License](https://img.shields.io/badge/license-MIT-green?style=flat-square)

---

## 📌 Overview

**SafeReport** is a full-stack web platform designed to empower women in Sri Lanka who have experienced online abuse, harassment, blackmail, domestic violence, stalking, or cyber crimes. It provides legal guidance, reporting tools, emotional support, and an AI-powered chatbot — all in a confidential and trauma-informed environment.

---

## ✨ Features

| Feature | Description |
|---|---|
| 🤖 **SafeHer AI Chatbot** | AI-powered support assistant scoped exclusively to women's safety topics |
| ⚖️ **Legal Guidance** | Sri Lanka-specific laws — Computer Crimes Act, Penal Code, PDVA |
| 📢 **Awareness** | Educational content on recognising and responding to abuse |
| 🗂️ **Case Reporting** | Guided incident reporting flow |
| 📰 **News** | Latest updates on women's rights and cyber safety in Sri Lanka |
| 🔒 **Confidential by Design** | No personal data collected during chat sessions |

---

## 🤖 SafeHer AI — How It Works

The chatbot uses a **two-step AI guard** to ensure it only responds to women's safety topics:

```
User Message
     │
     ▼
┌─────────────────────────────┐
│  STEP 1 — Topic Classifier  │  ← Groq API call (temp=0.0, max 5 tokens)
│  Returns only YES or NO     │
└─────────────────────────────┘
     │
     ├── NO  ──► Fixed off-topic reply returned instantly
     │
     └── YES ──►
          ▼
┌─────────────────────────────┐
│  STEP 2 — Support Response  │  ← Groq API call (Llama 3.1 8B Instant)
│  Warm, trauma-informed reply│
└─────────────────────────────┘
```

**Why two calls?** LLMs are trained to be helpful and will often answer off-topic questions even when instructed not to. By using a separate deterministic classifier first, off-topic messages (recipes, sports, coding, etc.) never reach the main model.

---

## 🏗️ Tech Stack

### Frontend
- **Angular 18+** — Standalone components
- **Bootstrap Icons** — UI icons
- **TypeScript** — Strict typing throughout

### Backend
- **ASP.NET Core 8** — REST API
- **C#** — Controllers and services
- **IHttpClientFactory** — HTTP client management

### AI
- **Groq API** — Ultra-fast LLM inference
- **Meta Llama 3.1 8B Instant** — The underlying language model

---

## 🚀 Getting Started

### Prerequisites

- Node.js 18+ and npm
- .NET 8 SDK
- A [Groq API key](https://console.groq.com/)

---

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/safereport.git
cd safereport
```

---

### 2. Backend Setup

```bash
cd WomenEmpower.API
```

Add your Groq API key to `appsettings.json`:

```json
{
  "Groq": {
    "ApiKey": "your_groq_api_key_here"
  }
}
```

Run the API:

```bash
dotnet restore
dotnet run
```

The API will start at `http://localhost:5032`.

---

### 3. Frontend Setup

```bash
cd safereport-frontend
npm install
ng serve
```

Open your browser at `http://localhost:4200`.

---

## 📁 Project Structure

```
safereport/
├── WomenEmpower.API/                  # ASP.NET Core backend
│   ├── Controllers/
│   │   └── ChatController.cs          # AI chatbot endpoint (two-step guard)
│   ├── appsettings.json               # Config (add Groq key here)
│   └── Program.cs
│
└── safereport-frontend/               # Angular frontend
    └── src/
        └── app/
            ├── home/
            │   ├── home.component.html
            │   └── home.component.css
            └── chatbot/
                ├── chatbot.component.ts
                ├── chatbot.component.html
                └── chatbot.component.css
```

---

## 📞 Emergency Contacts (Sri Lanka)

## 🛡️ Chatbot Topic Scope

SafeHer AI is strictly limited to:

- ✅ Emotional support for survivors
- ✅ Online abuse, harassment, stalking
- ✅ Blackmail and sextortion
- ✅ Fake accounts and impersonation
- ✅ Domestic and intimate partner violence
- ✅ Legal rights under Sri Lankan law
- ✅ Reporting to police, SLCERT, CID, MWCA
- ✅ Safety planning and evidence collection
- ❌ Everything else (food, sports, coding, general knowledge, etc.)

---

## 🤝 Contributing

Contributions are welcome! Please open an issue first to discuss what you'd like to change.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

---


## 💛 Acknowledgements

Built with compassion for every woman who deserves to feel safe, heard, and supported.

> *"A safe, powerful space built by women, for women. Your voice matters. Your safety matters. You matter."*
