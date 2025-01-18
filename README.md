# ast-diff-tool

The repository contains the implementation of the desktop application serving as the frontend of the C++ Tools, providing the input for the _Dump Tool_ and visualizing the output of the _Comparer Tool_ ([ast-tree-comparer](https://github.com/baguadam/ast-tree-comparer)). The application  is built in C# using Windows Presentation Foundation (WPF) that is a powerful framework building modern, desktop-based applications on Windows. It is a part of the .NET ecosystem. The application also connects to the Neo4j database using the Neo4j Driver (Neo4j.Driver NuGet package), leveraging the Bolt protocol developed by Neo4j to efficiently interact with the database. CommunityToolkit.Mvvm NeGet package is utilized to simplify the creation and managements of commands in the applications.

## Structure of the application
The application is developed withing the ASTDiffTool solution, organized into distinct project that separate the components. These projects are:
- **ASTDiffTool**: the main project of the application that contains the App.xaml, the AssemblyInfo.cs.
- **ASTDiffTool.Models**: a Class Library that defines the application’s model and represent the core data structure.
- **ASTDiffTool.Services**: a Class Library containing the service related implementations of the application such as communicating with the database, file operations, managing the C++ tools.
- **ASTDiffTool.Shared**: a Class Library containing common implementations and utility classes used throughout the application.
- **ASTDiffTool.ViewModels**: a Class Library containing the view model implementation.
- **ASTDiffTool.Views**: a WPF Class Library that contains the application’s views, user controls, styles, defining the visual structure and user interface.

## UI
### Main Page
The landing page of the application. It guides the user step-by-step to provide the necessary files for the C++ tools, and start exeecution of the tools. During the execution, it displays the current status, while creating the project structure in the background that contains the Dump Tool's output (the two ASTs (Abstarct Syntax Trees) in text-based format) alongside with the logs of the C++ tools.  

![newproject_start](https://github.com/user-attachments/assets/4815a529-2cf6-4e23-82f7-1f092274f981)
![newproject_during_execution](https://github.com/user-attachments/assets/598f44a9-1608-4307-9c2a-b3a79334976a)
![newproject_after_success](https://github.com/user-attachments/assets/97de0a1c-350b-468b-b79a-5a3b9d982b3f)

### Project Details Page
Statistics about the nodes in the database. The user can investigate the distribution of the detected differences by separate categories. 

![projectdetails](https://github.com/user-attachments/assets/ed937652-65f6-411a-a013-a79e79096ce7)

### Tree View Page
Tree-like display of the nodes. On the page, the nodes are displayed by the selected categori in a _TreeView_ control, maintaing their tree structure. The user can investigate the separate nodes with their subtrees. 

![treeview_basic](https://github.com/user-attachments/assets/935a1668-54b7-43c4-aeb9-a561f3b1a879)
