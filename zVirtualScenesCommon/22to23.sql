USE `zvirtualscenes`; 

CREATE  TABLE `scheduled_tasks` (

  `id` INT NOT NULL AUTO_INCREMENT ,

  `Name` VARCHAR(255) NOT NULL ,

  `Enabled` VARCHAR(45) NOT NULL ,

  `Scene_id` INT(11) NULL ,

  `Frequency` INT(11) NULL ,

  `RecurMonday` VARCHAR(45) NULL ,

  `RecurTuesday` VARCHAR(45) NULL ,

  `RecurWednesday` VARCHAR(45) NULL ,

  `RecurThursday` VARCHAR(45) NULL ,

  `RecurFriday` VARCHAR(45) NULL ,

  `RecurSaturday` VARCHAR(45) NULL ,

  `RecurSunday` VARCHAR(45) NULL ,

  `RecurDays` INT(11) NULL ,

  `RecurWeeks` INT(11) NULL ,

  `RecurMonth` INT(11) NULL ,

  `RecurDayofMonth` INT(11) NULL ,

  `RecurSeconds` INT(11) NULL ,

  `StartTime` VARCHAR(255) NULL ,

  `sort_order` INT(11) NULL ,

  PRIMARY KEY (`id`) ,

  UNIQUE INDEX `id_UNIQUE` (`id` ASC) );

UPDATE `version` SET `version`='2.3' WHERE `id`='1';