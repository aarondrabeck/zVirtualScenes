CREATE TABLE scenes (
  id             integer PRIMARY KEY IDENTITY NOT NULL,
  friendly_name  nvarchar(500) DEFAULT NULL,
  sort_order     integer DEFAULT NULL,
  is_running     bit NOT NULL DEFAULT 0
);
CREATE TABLE scheduled_tasks (
  id               integer PRIMARY KEY IDENTITY NOT NULL,
  friendly_name    nvarchar(500) NOT NULL,
  Enabled          bit NOT NULL,
  Scene_id         integer NOT NULL,
  Frequency        integer DEFAULT NULL,
  RecurMonday      bit DEFAULT NULL,
  RecurTuesday     bit DEFAULT NULL,
  RecurWednesday   bit DEFAULT NULL,
  RecurThursday    bit DEFAULT NULL,
  RecurFriday      bit DEFAULT NULL,
  RecurSaturday    bit DEFAULT NULL,
  RecurSunday      bit DEFAULT NULL,
  RecurDays        integer DEFAULT NULL,
  RecurWeeks       integer DEFAULT NULL,
  RecurMonth       integer DEFAULT NULL,
  RecurDayofMonth  integer DEFAULT NULL,
  RecurSeconds     integer DEFAULT NULL,
  StartTime        datetime DEFAULT NULL,
  sort_order       integer DEFAULT NULL
, RecurEven bit DEFAULT NULL, RecurDay01 bit DEFAULT NULL, RecurDay02 bit DEFAULT NULL, RecurDay03 bit DEFAULT NULL, RecurDay04 bit DEFAULT NULL, RecurDay05 bit DEFAULT NULL, RecurDay06 bit DEFAULT NULL, RecurDay07 bit DEFAULT NULL, RecurDay08 bit DEFAULT NULL, RecurDay09 bit DEFAULT NULL, RecurDay10 bit DEFAULT NULL, RecurDay11 bit DEFAULT NULL, RecurDay12 bit DEFAULT NULL, RecurDay13 bit DEFAULT NULL, RecurDay14 bit DEFAULT NULL, RecurDay15 bit DEFAULT NULL, RecurDay16 bit DEFAULT NULL, RecurDay17 bit DEFAULT NULL, RecurDay18 bit DEFAULT NULL, RecurDay19 bit DEFAULT NULL, RecurDay20 bit DEFAULT NULL, RecurDay21 bit DEFAULT NULL, RecurDay22 bit DEFAULT NULL, RecurDay23 bit DEFAULT NULL, RecurDay24 bit DEFAULT NULL, RecurDay25 bit DEFAULT NULL, RecurDay26 bit DEFAULT NULL, RecurDay27 bit DEFAULT NULL, RecurDay28 bit DEFAULT NULL, RecurDay29 bit DEFAULT NULL, RecurDay30 bit DEFAULT NULL, RecurDay31 bit DEFAULT NULL);




CREATE TABLE builtin_commands (
  id                        integer PRIMARY KEY IDENTITY NOT NULL,
  name                      nvarchar(500) NOT NULL,
  friendly_name             nvarchar(500) NOT NULL,
  arg_data_type             int NOT NULL,
  custom_data1              nvarchar(500) DEFAULT NULL,
  custom_data2              nvarchar(500) DEFAULT NULL,
  description               nvarchar(500) DEFAULT NULL,
  show_on_dynamic_obj_list  bit NOT NULL
, help nvarchar(500), sort_order integer);

CREATE TABLE builtin_command_que (
  id                  integer PRIMARY KEY IDENTITY NOT NULL,
  arg                 nvarchar(500) NOT NULL,
  builtin_command_id  integer NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (builtin_command_id)
    REFERENCES builtin_commands(id)
    ON DELETE CASCADE 
    ON UPDATE NO ACTION
);

CREATE TABLE builtin_command_options (
  id                  integer PRIMARY KEY IDENTITY NOT NULL,
  builtin_command_id  integer NOT NULL,
  name                nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (builtin_command_id)
    REFERENCES builtin_commands(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE db_info (
  id          integer PRIMARY KEY IDENTITY NOT NULL,
  info_name   nvarchar(500) NOT NULL,
  info_value  nvarchar(500) NOT NULL
);


CREATE TABLE plugins (
  id             integer PRIMARY KEY IDENTITY NOT NULL,
  friendly_name  nvarchar(500) NOT NULL,
  name           nvarchar(500) NOT NULL,
  enabled        bit NOT NULL DEFAULT 0,
  description    nvarchar(500) DEFAULT NULL
);

CREATE TABLE device_types (
  id             integer PRIMARY KEY IDENTITY NOT NULL,
  plugin_id      integer NOT NULL,
  name           nvarchar(500) NOT NULL,
  show_in_list   bit NOT NULL DEFAULT 1,
  friendly_name  nvarchar(500),
  /* Foreign keys */
  FOREIGN KEY (plugin_id)
    REFERENCES plugins(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE devices (
  id               integer PRIMARY KEY IDENTITY NOT NULL,
  device_type_id   integer NOT NULL,
  node_id          integer NOT NULL,
  friendly_name    nvarchar(500) NOT NULL,
  last_heard_from  datetime DEFAULT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_type_id)
    REFERENCES device_types(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE TABLE device_commands (
  id             integer PRIMARY KEY IDENTITY NOT NULL,
  device_id      integer NOT NULL,
  name           nvarchar(500) NOT NULL,
  friendly_name  nvarchar(500) NOT NULL,
  arg_data_type  integer NOT NULL,
  custom_data1   nvarchar(500) DEFAULT NULL,
  custom_data2   nvarchar(500) DEFAULT NULL,
  description    nvarchar(500) DEFAULT NULL,
  sort_order     integer DEFAULT NULL,
  help           nvarchar(500),
  /* Foreign keys */
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);


CREATE TABLE device_command_options (
  id                 integer PRIMARY KEY IDENTITY NOT NULL,
  device_command_id  integer NOT NULL,
  name               nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_command_id)
    REFERENCES device_commands(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);
CREATE TABLE device_command_que (
  id                 integer PRIMARY KEY IDENTITY NOT NULL,
  device_id          integer NOT NULL,
  device_command_id  integer NOT NULL,
  arg                nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION, 
  FOREIGN KEY (device_command_id)
    REFERENCES device_commands(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE TABLE device_propertys (
  id               integer PRIMARY KEY IDENTITY NOT NULL,
  friendly_name    nvarchar(500) NOT NULL,
  name             nvarchar(500) NOT NULL,
  default_value    nvarchar(500) NOT NULL,
  value_data_type  integer NOT NULL
);

CREATE TABLE device_property_options (
  id                  integer PRIMARY KEY IDENTITY NOT NULL,
  device_property_id  integer NOT NULL,
  name                nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_property_id)
    REFERENCES device_propertys(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
CREATE TABLE device_property_values (
  id                  integer PRIMARY KEY IDENTITY NOT NULL,
  device_id           integer NOT NULL,
  device_property_id  integer NOT NULL,
  value               nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_property_id)
    REFERENCES device_propertys(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE device_type_commands (
  id              integer PRIMARY KEY IDENTITY NOT NULL,
  device_type_id  integer NOT NULL,
  description     nvarchar(500) NOT NULL,
  friendly_name   nvarchar(500) NOT NULL,
  arg_data_type   integer NOT NULL,
  custom_data1    nvarchar(500) DEFAULT NULL,
  custom_data2    nvarchar(500) DEFAULT NULL,
  help            nvarchar(500) DEFAULT NULL,
  name            nvarchar(500),
  sort_order      integer,
  /* Foreign keys */
  FOREIGN KEY (device_type_id)
    REFERENCES device_types(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE device_type_command_options (
  id                      integer PRIMARY KEY IDENTITY NOT NULL,
  device_type_command_id  integer NOT NULL,
  options                  nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_type_command_id)
    REFERENCES device_type_commands(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);
CREATE TABLE device_type_command_que (
  id                      integer PRIMARY KEY IDENTITY NOT NULL,
  device_type_command_id  integer NOT NULL,
  device_id               integer NOT NULL,
  arg                     nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION, 
  FOREIGN KEY (device_type_command_id)
    REFERENCES device_type_commands(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE device_values (
  id              integer PRIMARY KEY IDENTITY NOT NULL,
  device_id       integer NOT NULL,
  value_id        nvarchar(500) DEFAULT NULL,
  label_name      nvarchar(500) DEFAULT NULL,
  genre           nvarchar(500) DEFAULT NULL,
  index2         nvarchar(500) DEFAULT NULL,
  type            nvarchar(500) DEFAULT NULL,
  commandClassId  nvarchar(500) DEFAULT NULL,
  value2           nvarchar(500) DEFAULT NULL, 
  read_only bit NOT NULL DEFAULT 1,
  /* Foreign keys */
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE device_value_triggers (
  id                integer PRIMARY KEY IDENTITY NOT NULL,
  device_value_id   integer NOT NULL,
  trigger_operator  int,
  trigger_value     nvarchar(500),
  enabled           bit NOT NULL,
  Name              nvarchar(500),
  scene_id          integer, trigger_type integer NOT NULL DEFAULT 0, 
  trigger_script nvarchar(500),
  /* Foreign keys */
  FOREIGN KEY (device_value_id)
    REFERENCES device_values(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION, 
  FOREIGN KEY (scene_id)
    REFERENCES scenes(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);

CREATE TABLE groups (
  id    integer PRIMARY KEY IDENTITY NOT NULL,
  name  nvarchar(500) NOT NULL
);

CREATE TABLE group_devices (
  device_id  integer NOT NULL,
  id         integer PRIMARY KEY IDENTITY NOT NULL,
  group_id   integer NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (group_id)
    REFERENCES groups(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION, 
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE plugin_settings (
  id               integer PRIMARY KEY IDENTITY NOT NULL,
  plugin_id        integer NOT NULL,
  friendly_name    nvarchar(500) NOT NULL,
  value            nvarchar(500) NOT NULL,
  value_data_type  integer NOT NULL,
  description      nvarchar(500) DEFAULT NULL,
  name             nvarchar(500),
  /* Foreign keys */
  FOREIGN KEY (plugin_id)
    REFERENCES plugins(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

CREATE TABLE plugin_setting_options (
  id                  integer PRIMARY KEY IDENTITY NOT NULL,
  plugin_settings_id  integer NOT NULL,
  options              nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (plugin_settings_id)
    REFERENCES plugin_settings(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);


CREATE TABLE program_options (
  id     integer PRIMARY KEY IDENTITY NOT NULL,
  name   nvarchar(500),
  value  nvarchar(500)
);
CREATE TABLE scene_commands (
  id               integer PRIMARY KEY IDENTITY NOT NULL,
  scene_id         integer NOT NULL,
  device_id        integer,
  command_type_id  integer NOT NULL,
  command_id       integer NOT NULL,
  arg              nvarchar(500) NOT NULL,
  sort_order       integer DEFAULT NULL,
  /* Foreign keys */
  FOREIGN KEY (scene_id)
    REFERENCES scenes(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION, 
  FOREIGN KEY (device_id)
    REFERENCES devices(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);
CREATE TABLE scene_property (
  id               integer PRIMARY KEY IDENTITY NOT NULL,
  friendly_name    nvarchar(500) NOT NULL,
  defualt_value    nvarchar(500) NOT NULL,
  value_data_type  integer NOT NULL,
  description      nvarchar(500) DEFAULT NULL
, name nvarchar(500));
CREATE TABLE scene_property_option (
  id                 integer PRIMARY KEY IDENTITY NOT NULL,
  scene_property_id  integer NOT NULL,
  options            nvarchar(500) NOT NULL,
  /* Foreign keys */
  FOREIGN KEY (scene_property_id)
    REFERENCES scene_property(id)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
CREATE TABLE scene_property_value (
  id                 integer PRIMARY KEY IDENTITY NOT NULL,
  scene_id           integer NOT NULL,
  scene_property_id  integer NOT NULL,
  value              nvarchar(500) DEFAULT NULL,
  /* Foreign keys */
  FOREIGN KEY (scene_property_id)
    REFERENCES scene_property(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION, 
  FOREIGN KEY (scene_id)
    REFERENCES scenes(id)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);
