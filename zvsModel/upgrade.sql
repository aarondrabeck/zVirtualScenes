db_version=2
CREATE TABLE javascript_triggers (id integer PRIMARY KEY IDENTITY NOT NULL, Name nvarchar(500) NULL, Script nvarchar(2000) NULL,  isEnabled bit NOT NULL DEFAULT 0);