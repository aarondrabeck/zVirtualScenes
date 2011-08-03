-- MySQL dump 10.13  Distrib 5.5.9, for Win32 (x86)
--
-- Host: localhost    Database: zvirtualscenes
-- ------------------------------------------------------
-- Server version	5.5.12

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `zvirtualscenes`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `zvirtualscenes` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `zvirtualscenes`;

--
-- Table structure for table `builtin_command_options`
--

DROP TABLE IF EXISTS `builtin_command_options`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `builtin_command_options` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `builtin_command_id` int(11) NOT NULL,
  `txt_option` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `builtin_command_options`
--

LOCK TABLES `builtin_command_options` WRITE;
/*!40000 ALTER TABLE `builtin_command_options` DISABLE KEYS */;
/*!40000 ALTER TABLE `builtin_command_options` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `builtin_commands`
--

DROP TABLE IF EXISTS `builtin_commands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `builtin_commands` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `txt_command_name` varchar(255) NOT NULL,
  `txt_command_friendly_name` varchar(255) NOT NULL,
  `param_type` int(11) NOT NULL,
  `txt_custom_data1` varchar(255) DEFAULT NULL,
  `txt_custom_data2` varchar(255) DEFAULT NULL,
  `txt_cmd_help` varchar(1024) DEFAULT NULL,
  `show_on_dynamic_obj_list` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `builtin_commands`
--

LOCK TABLES `builtin_commands` WRITE;
/*!40000 ALTER TABLE `builtin_commands` DISABLE KEYS */;
INSERT INTO `builtin_commands` VALUES (1,'REPOLL_ME','Repoll this Device',1,'','','This will force a repoll on an object.','False'),(2,'REPOLL_ALL','Repoll all Devices',0,'','','This will force a repoll on all objects.','False'),(3,'GROUP_ON','Turn Group On',3,'','','Activates a group.','False'),(4,'GROUP_OFF','Turn Group Off',3,'','','Deactivates a group.','False');
/*!40000 ALTER TABLE `builtin_commands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `command_types`
--

DROP TABLE IF EXISTS `command_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `command_types` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `txt_type` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `command_types`
--

LOCK TABLES `command_types` WRITE;
/*!40000 ALTER TABLE `command_types` DISABLE KEYS */;
INSERT INTO `command_types` VALUES (1,'ObjectType'),(2,'Object'),(3,'Builtin');
/*!40000 ALTER TABLE `command_types` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commands`
--

DROP TABLE IF EXISTS `commands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `commands` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_id` int(11) NOT NULL,
  `command_type_id` int(11) NOT NULL,
  `command_id` int(11) NOT NULL,
  `txt_arg` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commands`
--

LOCK TABLES `commands` WRITE;
/*!40000 ALTER TABLE `commands` DISABLE KEYS */;
/*!40000 ALTER TABLE `commands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `event_scripts`
--

DROP TABLE IF EXISTS `event_scripts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `event_scripts` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_id` int(11) NOT NULL,
  `object_type_event_id` int(11) NOT NULL,
  `txt_event_name` varchar(45) NOT NULL,
  `txt_script` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `event_scripts`
--

LOCK TABLES `event_scripts` WRITE;
/*!40000 ALTER TABLE `event_scripts` DISABLE KEYS */;
/*!40000 ALTER TABLE `event_scripts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `group_objects`
--

DROP TABLE IF EXISTS `group_objects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `group_objects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_id` int(11) NOT NULL,
  `group_id` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `group_objects`
--

LOCK TABLES `group_objects` WRITE;
/*!40000 ALTER TABLE `group_objects` DISABLE KEYS */;
/*!40000 ALTER TABLE `group_objects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `groups`
--

DROP TABLE IF EXISTS `groups`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `groups` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `txt_group_name` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `groups`
--

LOCK TABLES `groups` WRITE;
/*!40000 ALTER TABLE `groups` DISABLE KEYS */;
/*!40000 ALTER TABLE `groups` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_commands`
--

DROP TABLE IF EXISTS `object_commands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_commands` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_id` int(11) NOT NULL,
  `txt_command_name` varchar(255) NOT NULL,
  `txt_command_friendly_name` varchar(255) NOT NULL,
  `param_type` int(11) NOT NULL,
  `txt_custom_data1` varchar(255) DEFAULT NULL,
  `txt_custom_data2` varchar(255) DEFAULT NULL,
  `txt_cmd_help` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_commands`
--

LOCK TABLES `object_commands` WRITE;
/*!40000 ALTER TABLE `object_commands` DISABLE KEYS */;
/*!40000 ALTER TABLE `object_commands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_commands_options`
--

DROP TABLE IF EXISTS `object_commands_options`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_commands_options` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_command_id` int(11) NOT NULL,
  `txt_option` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_commands_options`
--

LOCK TABLES `object_commands_options` WRITE;
/*!40000 ALTER TABLE `object_commands_options` DISABLE KEYS */;
/*!40000 ALTER TABLE `object_commands_options` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_properties`
--

DROP TABLE IF EXISTS `object_properties`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_properties` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `txt_property_friendly_name` varchar(1024) NOT NULL,
  `txt_property_name` varchar(255) NOT NULL,
  `txt_default_value` varchar(45) NOT NULL,
  `property_type` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`),
  UNIQUE KEY `txt_property_name_UNIQUE` (`txt_property_name`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_properties`
--

LOCK TABLES `object_properties` WRITE;
/*!40000 ALTER TABLE `object_properties` DISABLE KEYS */;
INSERT INTO `object_properties` VALUES (1,'Enable polling for this device.','ENABLEPOLLING','false',5),(2,'Show in Lightswitch List','SHOWINLSLIST','true',5),(3,'Level that an object is set to when using the on command.','DEFAULONLEVEL','99',4);
/*!40000 ALTER TABLE `object_properties` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_property_options`
--

DROP TABLE IF EXISTS `object_property_options`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_property_options` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_property_id` int(11) NOT NULL,
  `txt_option` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_property_options`
--

LOCK TABLES `object_property_options` WRITE;
/*!40000 ALTER TABLE `object_property_options` DISABLE KEYS */;
/*!40000 ALTER TABLE `object_property_options` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_property_settings`
--

DROP TABLE IF EXISTS `object_property_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_property_settings` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_id` int(11) NOT NULL,
  `object_property_id` int(11) NOT NULL,
  `txt_property_value` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_property_settings`
--

LOCK TABLES `object_property_settings` WRITE;
/*!40000 ALTER TABLE `object_property_settings` DISABLE KEYS */;
/*!40000 ALTER TABLE `object_property_settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_type_commands`
--

DROP TABLE IF EXISTS `object_type_commands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_type_commands` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_type_id` int(11) NOT NULL,
  `txt_command_name` varchar(255) NOT NULL,
  `txt_command_friendly_name` varchar(255) NOT NULL,
  `param_type` int(11) NOT NULL,
  `txt_custom_data1` varchar(255) DEFAULT NULL,
  `txt_custom_data2` varchar(255) DEFAULT NULL,
  `txt_cmd_help` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_type_commands`
--

LOCK TABLES `object_type_commands` WRITE;
/*!40000 ALTER TABLE `object_type_commands` DISABLE KEYS */;
INSERT INTO `object_type_commands` VALUES (1,3,'RESET','Reset Controller',0,'','','Earses all Z-Wave netowrks from your controller.'),(2,3,'ADDDEVICE','Add Device to Network',0,'','','Puts your controller in a mode to wait for new devices.'),(3,4,'TURNON','Turn On',0,'','','Sets a switch to 100%'),(4,4,'TURNOFF','Turn Off',0,'','','Sets a switch to 0%'),(5,5,'TURNON','Turn On',0,'','','Sets a dimmer to 100%'),(6,5,'TURNOFF','Turn Off',0,'','','Sets a dimmer to 0%'),(7,5,'SETPRESETLEVEL','Set Level',7,'','','Sets a dimmer to a preset level.'),(8,6,'SETENERGYMODE','Set Energy Mode',0,'','','Set thermosat to Energy Mode.'),(9,6,'SETCONFORTMODE','Set Confort Mode',0,'','','Set thermosat to Confort Mode. (Run)'),(10,7,'SAY','Say',3,'','','Used to make the computer say any command you want');
/*!40000 ALTER TABLE `object_type_commands` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_type_commands_options`
--

DROP TABLE IF EXISTS `object_type_commands_options`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_type_commands_options` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_type_command_id` int(11) NOT NULL,
  `txt_option` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_type_commands_options`
--

LOCK TABLES `object_type_commands_options` WRITE;
/*!40000 ALTER TABLE `object_type_commands_options` DISABLE KEYS */;
INSERT INTO `object_type_commands_options` VALUES (1,7,'0%'),(2,7,'20%'),(3,7,'40%'),(4,7,'60%'),(5,7,'80%'),(6,7,'100%'),(7,7,'255');
/*!40000 ALTER TABLE `object_type_commands_options` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_type_events`
--

DROP TABLE IF EXISTS `object_type_events`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_type_events` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_type_id` int(11) NOT NULL,
  `txt_event_name` varchar(45) NOT NULL,
  `txt_event` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_type_events`
--

LOCK TABLES `object_type_events` WRITE;
/*!40000 ALTER TABLE `object_type_events` DISABLE KEYS */;
/*!40000 ALTER TABLE `object_type_events` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_types`
--

DROP TABLE IF EXISTS `object_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_types` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `plugin_id` int(11) NOT NULL,
  `txt_object_type` varchar(45) NOT NULL,
  `show_in_list` tinyint(4) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_types`
--

LOCK TABLES `object_types` WRITE;
/*!40000 ALTER TABLE `object_types` DISABLE KEYS */;
INSERT INTO `object_types` VALUES (1,1,'JABBER',0),(2,2,'LIGHTSWITCH',0),(3,3,'CONTROLLER',1),(4,3,'SWITCH',1),(5,3,'DIMMER',1),(6,3,'THERMOSTAT',1),(7,4,'SPEECH',0);
/*!40000 ALTER TABLE `object_types` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `object_values`
--

DROP TABLE IF EXISTS `object_values`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `object_values` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ObjectId` int(11) NOT NULL,
  `txt_value_id` varchar(45) DEFAULT NULL,
  `txt_label_name` varchar(45) DEFAULT NULL,
  `txt_genre` varchar(45) DEFAULT NULL,
  `txt_index` varchar(45) DEFAULT NULL,
  `txt_type` varchar(45) DEFAULT NULL,
  `txt_commandclassid` varchar(45) DEFAULT NULL,
  `txt_value` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `object_values`
--

LOCK TABLES `object_values` WRITE;
/*!40000 ALTER TABLE `object_values` DISABLE KEYS */;
/*!40000 ALTER TABLE `object_values` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `objects`
--

DROP TABLE IF EXISTS `objects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `objects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `object_type_id` int(11) NOT NULL,
  `node_id` int(11) NOT NULL,
  `txt_object_name` varchar(45) NOT NULL,
  `last_heard_from` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `objects`
--

LOCK TABLES `objects` WRITE;
/*!40000 ALTER TABLE `objects` DISABLE KEYS */;
INSERT INTO `objects` VALUES (1,1,1,'JABBER',NULL),(2,2,1,'LIGHTSWITCH',NULL),(3,7,1,'SPEECH',NULL);
/*!40000 ALTER TABLE `objects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plugin_settings`
--

DROP TABLE IF EXISTS `plugin_settings`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `plugin_settings` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `plugin_id` int(11) NOT NULL,
  `txt_setting_name` varchar(255) NOT NULL,
  `txt_setting_value` varchar(1024) NOT NULL,
  `plugin_settings_type_id` int(11) NOT NULL,
  `txt_setting_description` varchar(1024) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plugin_settings`
--

LOCK TABLES `plugin_settings` WRITE;
/*!40000 ALTER TABLE `plugin_settings` DISABLE KEYS */;
INSERT INTO `plugin_settings` VALUES (1,1,'Jabber Server','gmail.com',3,'The Jabber server to connect to.'),(2,1,'Jabber Username','user',3,'The username of the jabber user.'),(3,1,'Jabber Password','passw0rd',3,'The password of the jabber user.'),(4,1,'Send to','user@gmail.com',3,'Jabber users that will receive notifications. (comma seperated)'),(5,1,'Verbose Logging','true',5,'(Writes all server client communication to the log for debugging.)'),(6,1,'Notifications to send','DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State',3,'Include all values you would like announced. Comma Seperated.'),(7,2,'Port','1337',1,'LightSwitch will listen for connections on this port.'),(8,2,'Max Conn.','200',1,'The maximum number of connections allowed.'),(9,2,'Verbose Logging','true',5,'(Writes all server client communication to the log for debugging.)'),(10,2,'Password','ChaNgeMe444',3,'The password clients must use to connect to the LightSwitch server. '),(11,2,'Sort Device List','true',5,'(Alphabetically sorts the device list.)'),(12,3,'Com Port','7',1,'The COM port that your z-wave controller is assigned to.'),(13,3,'Use HID','false',5,'Use HID rather than COM port. (use this for ControlThink Sticks)'),(14,3,'Polling Interval','360',1,'The frequency in which devices are polled for level status on your network.  Set high to avoid excessive network  traffic. '),(15,4,'Enable announce on','Level',7,'Select the values to annouce.'),(16,4,'Announce on custom values','DIMMER:Basic, THERMOSTAT:Temperature, SWITCH:Basic, THERMOSTAT:Operating State',3,'Include all values you would like announced. Comma Seperated.');
/*!40000 ALTER TABLE `plugin_settings` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plugin_settings_options`
--

DROP TABLE IF EXISTS `plugin_settings_options`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `plugin_settings_options` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `plugin_settings_id` int(11) NOT NULL,
  `txt_option` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plugin_settings_options`
--

LOCK TABLES `plugin_settings_options` WRITE;
/*!40000 ALTER TABLE `plugin_settings_options` DISABLE KEYS */;
INSERT INTO `plugin_settings_options` VALUES (1,15,'Switch Level'),(2,15,'Dimmer Level'),(3,15,'Thermostat Operating State and Temp'),(4,15,'All of the above'),(5,15,'Custom');
/*!40000 ALTER TABLE `plugin_settings_options` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plugins`
--

DROP TABLE IF EXISTS `plugins`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `plugins` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `txt_plugin_name` varchar(45) NOT NULL,
  `txt_api_name` varchar(45) NOT NULL,
  `enabled` tinyint(4) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plugins`
--

LOCK TABLES `plugins` WRITE;
/*!40000 ALTER TABLE `plugins` DISABLE KEYS */;
INSERT INTO `plugins` VALUES (1,'Jabber','JABBER',0),(2,'LightSwitch','LIGHTSWITCH',0),(3,'OpenZWave','OPENZWAVE',0),(4,'Speech','SPEECH',0);
/*!40000 ALTER TABLE `plugins` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `version`
--

DROP TABLE IF EXISTS `version`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `version` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `version` varchar(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id_UNIQUE` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `version`
--

LOCK TABLES `version` WRITE;
/*!40000 ALTER TABLE `version` DISABLE KEYS */;
INSERT INTO `version` VALUES (1,'2.0 Base');
/*!40000 ALTER TABLE `version` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2011-07-06 18:03:25
