-- 會員表索引
CREATE INDEX idx_member_email ON Members(Email);
CREATE INDEX idx_member_username ON Members(Username);

-- 群組表索引
CREATE INDEX idx_group_name ON Groups(Name);

-- 會員群組關聯表索引
CREATE INDEX idx_member_group_member_id ON MemberGroups(MemberId);
CREATE INDEX idx_member_group_group_id ON MemberGroups(GroupId);