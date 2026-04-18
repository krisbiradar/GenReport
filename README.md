# GenReport
> AI + BI — one-click business intelligence, on your server.

GenReport is a self-hosted, all-in-one business intelligence platform powered by AI. It bundles a **.NET Core** backend, a **React** frontend, and a **Go** service layer into a single binary — no Docker, no cloud subscriptions, no complex setup. Just download, run, and start turning your data into insights.

---

## ✨ Features

- **One-click installation** — download and run a single binary, no dependencies to manage
- **AI-assisted reporting** — natural language queries, smart chart suggestions, and automated summaries
- **Built-in BI tooling** — dashboards, reports, data connectors, and visualizations out of the box
- **Self-hosted & private** — your data never leaves your server
- **Bundled stack** — .NET Core + React + Go, pre-compiled and ready to go
- **Fast & lightweight** — Go-powered service layer keeps things snappy even on modest hardware

---

## 🖥️ Platform Support

| Platform | Status |
|---|---|
| macOS — Apple Silicon (M1/M2/M3) | ✅ Available |
| macOS — Intel | 🔜 Coming soon |
| Linux | 🔜 Coming soon |
| Windows | 🔜 Coming soon |

---

## 🚀 Quick Start (macOS Apple Silicon)

### Download

```bash
curl -L https://github.com/krisbiradar/Genreport.Installer/releases/download/v1.0.2/Genreport.pkg \
  -o genreport
chmod +x genreport
```

### Run

```bash
./genreport
```

GenReport will start on **http://localhost:2905** by default.

### First login

Open your browser and navigate to `http://localhost:2905`. An admin account is automatically created on first launch with the following default credentials:

| Field | Default |
|---|---|
| Email | `admin@organization.com` |
| Password | `AdminPassword123` |

> ⚠️ **Change your password immediately after first login.**

---

## 🏗️ Architecture

GenReport is a single self-contained binary that bundles three components:

```
genreport (binary)
├── .NET Core   → Business logic, data connectors, report engine
├── React       → Web UI, dashboards, chart builder
└── Go          → HTTP server, process orchestration, file serving
```

The Go layer boots first and manages the .NET Core runtime and React assets internally — users interact with a single process on a single port.

---

## 🗺️ Roadmap

- [x] macOS Apple Silicon
- [ ] macOS Intel
- [ ] Linux (x86_64, ARM64)
- [ ] Windows
- [ ] Docker image
- [ ] PostgreSQL support
- [ ] Multi-user & team workspaces
- [ ] LDAP / SSO authentication
- [ ] Scheduled reports & email delivery

---

## 🔗 Related Repositories

| Repo | Description |
|---|---|
| [GenReport.Go](https://github.com/krisbiradar/GenReport.Go) | Go service layer — HTTP server & process orchestration |
| [GenReport.ClientWebsite](https://github.com/krisbiradar/GenReport.ClientWebsite) | React frontend — dashboards, chart builder & web UI |
| [GenReport.Installer](https://github.com/krisbiradar/Genreport.Installer) | Installer & release packaging |

---

## 🤝 Contributing

Contributions are welcome! Please open an issue first to discuss what you'd like to change.

```bash
git clone https://github.com/krisbiradar/GenReport
cd genreport
```

---

## 📄 License

[MIT](LICENSE)

---

<p align="center">Made with ♥ by Kris & AI</p>
