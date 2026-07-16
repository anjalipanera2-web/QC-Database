# Paperless QC Database
This project was created in a 7-week long internship at Inteplast Group: Amtopp Stretch Films in collaboration with the plant's Quality Control (QC) and Operator (OP) department. This project is a ASP.NET website with a SQL database that serves as a prototype replacement for the 100% paper documentation system currently in place at the plant.

## Table of Contents
1. [Repository Structure](https://github.com/anjalipanera2-web/QC-Database/main/README.md#repository-structure)
2. [Website Features](https://github.com/anjalipanera2-web/QC-Database/main/README.md#website-features)
3. [Technologies Used](https://github.com/anjalipanera2-web/QC-Database/main/README.md#technologies-used)
4. [Future Improvements](https://github.com/anjalipanera2-web/QC-Database/main/README.md#future-improvements)
5. [Lessons Learned](https://github.com/anjalipanera2-web/QC-Database/main/README.md#lessons-learned)

## Repository Structure
- Database Architecture: Houses the SQL .dacpac file
- Website: Houses the files for the website
- .gitignore: Has the local files that Git should ignore when uploading to Github
- ProductionSheet.slnx: The solution file that runs the website=

## Website Features
The website has the following features

## Login Page
This page allows the user to select who they want to log in as. Currently, there is no password system in place.

## Home Page
This page has four separate tabs:
- QC - Quality Control: Houses the Stretch Film Audit Log, Stretch Film Test Result Log, and the Daily Quality Issues Log
- OP - Operator: Houses the Operator Self Inspection Log
- Databases: Allows users to view the saved Test Logs, Audit Logs, and Operator Logs
- Settings: Allows users to add custom film types and specifications, customize their autosave settings, and manage users

## QC - Quality Control Forms
### Stretch Film Audit Log
- Flag button to report any outstanding quality issues detected on roll
- Interactive form that models paper Audit Log to record details about each line audit
- Save button allows user to save log to corresponding database
- Export Excel and PDF buttons allow users to export the log as .xlsx or .pdf file

### Stretch Film Test Result Log
- Flag button to report any outstanding quality issues detected on roll
- Specification set that automatically generated based on data entered in header of sheet
- Iterative rows for users to input test details that are automatically compared to generated specification set
- Save button allows user to save log to corresponding database
- Export Excel and PDF buttons allow users to export the log as .xlsx or .pdf file

### Daily Quality Issues Log
- Form that summarizes the shift, crew, film type, and quality issues detected from Stretch Film Test and Audit Logs
- Automatically generated and drawn from test and audit logs from that given date

# OP - Operator Forms
## Operator Self Inspection Form
- Flag button to report any outstanding quality issues detected on roll
- Iterative rows for users to input audit details 
- Save button allows user to save log to corresponding database
- Export Excel and PDF buttons allow users to export the log as .xlsx or .pdf file

# Databases
- Allows users to view saved test, audit and operator logs
- Search bar allows users to search for any saved logs by date, crew, shift, film type, or any other metric
- Every user can view all uploaded logs
- Admins can edit and delete all logs
- Users can only edit and delete the logs they create
- Flagged logs have a flag beside them in the list view

# Settings
## Film Type and Formula Settings
- Custom film types can be added and deleted by admins via a drop-down menu
- Custom Specification rule sets can be added, viewed, edited, and deleted by admins

## Autosave settings
- Users can adjust autosave settings to occur every N seconds or after every edit

## Manage Users
- Admins can add new users or create new admins

# Technologies Used
Claude Code was supplementally utilized in the development process to understand the structure and syntax of the following languages:
- HTML
- C#
- CSS

# Future Improvements
- The Stretch Film Test Result Log is very developed, but the other forms need to be updated to have the same features the Test Log has
- The Daily Quality Issues Log needs to be futher developed

# Lessons Learned
- This was my first time coding a website in HTML and with a C# backend (with my primary coding language and experience being in Python), so I had some assistance from Claude Code to learn the structure and syntax of HTML and C#, as well as changing any backend dependencies when needed. Overall, however, I feel like I learned a great deal about how to use Blazor to make a ASP.NET website and about the general top-down workflow behind application development
- This is also my first experience using Github to document and upload my code, so this experience helped familiarize me with how Github works and how to commit changes to a repository. 
