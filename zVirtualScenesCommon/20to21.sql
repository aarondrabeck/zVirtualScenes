USE `zvirtualscenes`;CREATE  TABLE `zvirtualscenes`.`scenes_cmds` (
  `id` INT(11) NOT NULL AUTO_INCREMENT ,
  `scene_id` INT(11) NOT NULL ,
  `object_id` INT(11) NOT NULL ,
  `command_type_id` INT(11) NOT NULL ,
  `command_id` INT(11) NOT NULL ,
  `txt_arg` VARCHAR(45) NOT NULL ,
  `sort_order` INT(11) NULL ,
  PRIMARY KEY (`id`) ,
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) );

CREATE  TABLE `zvirtualscenes`.`scenes` (
  `id` INT(11) NOT NULL AUTO_INCREMENT ,
  `txt_name` VARCHAR(255) NULL ,
`sort_order` INT(11) NULL ,
`is_running` VARCHAR(45) NULL ,
  PRIMARY KEY (`id`) ,
  UNIQUE INDEX `id_UNIQUE` (`id` ASC) );    UPDATE `zvirtualscenes`.`version` SET `version`='2.1' WHERE `id`='1';