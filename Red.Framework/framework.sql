USE `DATABASE_NAME_HERE`;

CREATE TABLE IF NOT EXISTS `characters` (
    `character_id` INT(10) NOT NULL AUTO_INCREMENT,
    `license` VARCHAR(200) NOT NULL DEFAULT '0',
    `first_name` VARCHAR(50) DEFAULT NULL,
    `last_name` VARCHAR(50) DEFAULT NULL,
    `dob` VARCHAR(50) DEFAULT NULL,
    `gender` VARCHAR(50) DEFAULT NULL,
    `cash` INT(10) DEFAULT '0',
    `bank` INT(10) DEFAULT '0',
    `department` VARCHAR(50) DEFAULT NULL,
    PRIMARY KEY (`character_id`) USING BTREE
);