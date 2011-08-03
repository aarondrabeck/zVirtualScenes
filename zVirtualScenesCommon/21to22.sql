USE `zvirtualscenes`;
ALTER TABLE `object_commands` ADD COLUMN `sort_order` INT(11) NULL  AFTER `txt_cmd_help` ;
UPDATE `version` SET `version`='2.2' WHERE `id`='1';