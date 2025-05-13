# Git Setup and Usage Guide

## Initial Setup

### First-time Git setup
```bash
git config --global user.name "your_username"
git config --global user.email "your_email@example.com"
```

## Basic Commands

### Daily Workflow
1. Before starting work:
```bash
git pull
```

2. After making changes:
```bash
git add .
git commit -m "Description of changes"
git push
```

### Common Scenarios

#### Checking Status
To see what files have been changed:
```bash
git status
```

#### Viewing History
To see commit history:
```bash
git log
```

#### Creating New Branch
```bash
git checkout -b branch_name
```

## Best Practices
- Always pull before starting work
- Commit frequently with clear messages
- Keep sensitive data out of git (use .gitignore)
- Resolve conflicts carefully

## Project Structure
- `/docs` - Documentation
- `/Services` - Backend services
- `/Views` - Frontend views
- `/Models` - Data models
- `/Styles` - UI styles 