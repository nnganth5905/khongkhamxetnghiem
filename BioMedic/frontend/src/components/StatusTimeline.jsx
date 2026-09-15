import React from 'react';

export default function StatusTimeline({
  steps = [],
  currentStep = 0,
}) {
  return (
    <div className="status-timeline">
      {steps.map(
        (
          step,
          index
        ) => {
          const completed =
            index <
            currentStep;

          const active =
            index ===
            currentStep;

          return (
            <div
              key={
                step.id ??
                index
              }
              className={`status-timeline-item ${
                completed
                  ? 'completed'
                  : ''
              } ${
                active
                  ? 'active'
                  : ''
              }`}
            >
              <div className="status-timeline-marker">
                <div className="status-timeline-circle">
                  {completed ? (
                    <i className="fa-solid fa-check" />
                  ) : (
                    index + 1
                  )}
                </div>

                {index <
                  steps.length -
                    1 && (
                  <div className="status-timeline-line" />
                )}
              </div>

              <div className="status-timeline-content">
                <div className="fw-semibold">
                  {
                    step.title
                  }
                </div>

                {step.description && (
                  <div className="small text-secondary mt-1">
                    {
                      step.description
                    }
                  </div>
                )}

                {step.time && (
                  <div className="small text-muted mt-1">
                    <i className="fa-regular fa-clock me-1" />

                    {
                      step.time
                    }
                  </div>
                )}
              </div>
            </div>
          );
        }
      )}
    </div>
  );
}