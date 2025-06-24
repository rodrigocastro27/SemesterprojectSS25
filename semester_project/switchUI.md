## 🔄 UI Update Instructions

> **Note**: All UI migration work is being done in the `feature/switchedUI` branch.


The user interface for this application is undergoing a redesign. All new UI components and pages are located in the `newUI/` folder.

### Migration Guidelines

- **Sequential Replacement**: Due to numerous interdependencies and import paths across the project, it is **strongly recommended** to replace the old UI components **one page at a time** and in **chronological order**. This will help prevent unexpected breakages and maintain project stability.
- **New Assets**: Any new images or media files required for the redesigned UI should be added to the `assets/images/` directory. Please avoid placing new image assets elsewhere to maintain consistency and organisation.

### Folder Structure
```
project-root/
├── assets/
│ └── images/         # Place all new images here
├── lib/
│ └── newUI/          # Contains the new UI components/pages
└── ...               # Other existing project files
```
Please follow these guidelines carefully to ensure a smooth transition to the new user interface.  
