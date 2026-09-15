package com.biomedic.backend.repository;

import com.biomedic.backend.entity.Role;

import java.util.Arrays;
import java.util.List;
import java.util.Optional;

/**
 * Role hiện đang là enum, không phải @Entity.
 *
 * Vì vậy repository này chỉ cung cấp helper mặc định và không truy cập DB.
 * Nếu schema thật có bảng role/vaitro riêng, file này sẽ được đổi sang
 * Spring Data JPA ở bước map database.
 */
public interface RoleRepository {

    default List<Role> findAll() {
        return Arrays.asList(
                Role.values()
        );
    }

    default Optional<Role> findByName(
            String value
    ) {
        if (
            value == null ||
            value.isBlank()
        ) {
            return Optional.empty();
        }

        return Optional.of(
                Role.fromDatabaseValue(value)
        );
    }
}