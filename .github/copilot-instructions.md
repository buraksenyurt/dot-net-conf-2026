# Copilot Instructions for dot-net-conf-2026

## Repository Overview

This is a **presentation/documentation repository** for a .NET Conference 2026 talk about AI-powered legacy system modernization. The repository contains a detailed case study written in Turkish describing an enterprise's journey migrating a legacy .NET Framework DMS (Dealer Management System) to modern technologies using AI assistance.

## Purpose

The primary content is `README.md`, which serves as both a conference presentation document and a detailed technical case study. This repository documents the approach, experiences, and results of using AI tools (primarily Claude Sonnet, GPT, Gemini) for modernizing a 5+ million LOC legacy system.

## Content Language

**All content is in Turkish (Türkçe)**. When working in this repository:
- Maintain Turkish language for all documentation
- Keep technical terminology consistent with existing usage
- Preserve formal business/technical writing style used throughout the document

## Documentation Structure

The README follows this narrative structure:
1. **Legacy System** - Description of the original DMS (5M+ LOC, .NET Framework 4.8, SQL Server, N-Tier architecture)
2. **Initial Modernization (2020-2024)** - Pre-AI modernization efforts (dependency injection, unit tests, CI/CD improvements)
3. **Level 0: First Stage** - AI-powered PoC using Claude Sonnet, Vue 3/Vite, .NET 9, PostgreSQL
4. **Development Process** - Spec-Oriented approach with AI agents and structured documentation
5. **Results and Future Plans** - 40% time reduction, RAG implementation plans, MCP protocol usage

## Key Technical Concepts

### Legacy Stack (Being Migrated From)
- .NET Framework 4.8, ASP.NET Web Forms, ADO.NET
- Microsoft SQL Server
- WCF, SOAP, REST services
- RabbitMQ messaging
- SSRS, Liquid reports
- Azure DevOps (CI/CD)

### Modern Stack (PoC Target)
- .NET 9, Vue.js 3, Vite
- Bootstrap 5 (UI Framework)
- PostgreSQL
- Entity Framework, Dapper
- Serilog (Logging)
- Keycloak (auth)
- GitHub Actions (CI/CD)
- Sonarqube

### AI Development Approach
- **Spec-Oriented Development** - Uses structured markdown documentation in `docs/` folder:
  - `docs/ui/`: HTML Wireframes for UI generation (Source of Truth for Frontend)
  - `docs/business/`: User Stories and Acceptance Criteria
  - `docs/domain-model/`: Entities and Value Objects
- **GitHub Copilot Agents** - Custom agents for different roles (Senior Developer, UI/UX Expert, Business Analyst, DevOps, QA)
- **MCP (Model Context Protocol)** - Custom MCP server for domain-specific interactions
- **Template-based generation** - dotnet templates and CLI tools for project scaffolding

## Editing Guidelines

### When updating metrics/statistics:
- Keep table formatting consistent
- Use Turkish number formatting (periods for thousands: 5.000.000+)
- Maintain alignment with existing metric categories

### When adding new sections:
- Follow the existing heading hierarchy (##, ###)
- Use similar subheadings: "Genel", "Teknik", "Metrikler", "Deneyimler"
- Include concrete numbers/metrics where applicable
- Maintain the balanced technical depth (detailed enough for developers, accessible for managers)

### When documenting AI tools/approaches:
- Always mention which AI model was used (Claude Sonnet, GPT, etc.)
- Include both successes and challenges/risks
- Reference the "Riskler" (Risks) section style for balanced perspectives

## Common Terminology

Maintain consistent Turkish technical terms:
- **Modernizasyon** - Modernization
- **Legacy sistem** - Legacy system
- **Teknik borç** - Technical debt
- **Yapay zeka asistanları** - AI assistants
- **Bayi yönetimi sistemi (DMS)** - Dealer Management System
- **Halusinasyon** - Hallucination (AI)
- **Spec-Oriented yaklaşım** - Spec-Oriented approach
- **PoC (Proof of Concept)** - Keep as PoC with Turkish explanation in parentheses
- **RAG (Retrieval Augmented Generation)** - Keep acronym with Turkish context

## Git Practices

This repository follows standard practices:
- Main branch for stable content
- Feature branches for major content additions
- Conventional commits encouraged for documentation changes

## Notes

- **Demo application** exists in `src/` folder (backend: .NET 9 Clean Architecture, frontend: Vue 3 + Vite + TypeScript)
- The repository contains both presentation documentation (README.md) and a working demo application
- Frontend uses Yarn package manager (not npm) due to Windows native binding compatibility
- The .gitignore is configured for Visual Studio/.NET projects
- Content is meant for conference presentation and public knowledge sharing
