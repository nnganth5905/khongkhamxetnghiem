package com.biomedic.backend.exception;

public class ResourceNotFoundException
        extends RuntimeException {

    public ResourceNotFoundException(
            String message
    ) {
        super(message);
    }

    public ResourceNotFoundException(
            String resource,
            String id
    ) {
        super(
                resource
                        + " không tồn tại với ID: "
                        + id
        );
    }
}