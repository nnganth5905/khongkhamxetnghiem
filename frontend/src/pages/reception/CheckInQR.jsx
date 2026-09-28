import React, {
  useEffect,
  useRef,
  useState,
} from 'react';

import Notification from '../../components/Notification';

import visitService from '../../services/visitService';

import {
  getApiErrorMessage,
} from '../../services/api';

export default function CheckInQR() {
  const videoRef =
    useRef(null);

  const streamRef =
    useRef(null);

  const detectorTimerRef =
    useRef(null);

  const busyRef =
    useRef(false);

  const mountedRef =
    useRef(true);

  const [
    manualCode,
    setManualCode,
  ] = useState('');

  const [
    cameraOn,
    setCameraOn,
  ] = useState(false);

  const [
    processing,
    setProcessing,
  ] = useState(false);

  const [
    message,
    setMessage,
  ] = useState({
    type: '',
    text: '',
  });

  // =====================================================
  // STOP CAMERA
  // =====================================================

  const stopCamera = (
    updateState = true
  ) => {
    if (
      detectorTimerRef.current
    ) {
      clearInterval(
        detectorTimerRef.current
      );

      detectorTimerRef.current =
        null;
    }

    if (
      streamRef.current
    ) {
      streamRef.current
        .getTracks()
        .forEach(
          (track) =>
            track.stop()
        );

      streamRef.current =
        null;
    }

    if (
      videoRef.current
    ) {
      videoRef.current
        .srcObject =
        null;
    }

    if (
      updateState &&
      mountedRef.current
    ) {
      setCameraOn(
        false
      );
    }
  };

  // =====================================================
  // PROCESS CODE
  // =====================================================

  const processCode =
    async (
      rawCode
    ) => {
      const code =
        String(
          rawCode ||
          ''
        ).trim();

      if (
        !code ||
        busyRef.current
      ) {
        return;
      }

      busyRef.current =
        true;

      setProcessing(
        true
      );

      try {
        const data =
          await visitService
            .checkInByQr(
              code
            );

        setMessage({
          type:
            'success',

          text:
            data?.message
            ||
            `Check-in thành công. Số thứ tự: ${
              data?.queueNumber ??
              '—'
            }.`,
        });

        setManualCode('');

        stopCamera();
      } catch (err) {
        setMessage({
          type:
            'danger',

          text:
            getApiErrorMessage(
              err,
              'Mã QR không hợp lệ hoặc không thể check-in.'
            ),
        });
      } finally {
        busyRef.current =
          false;

        if (
          mountedRef.current
        ) {
          setProcessing(
            false
          );
        }
      }
    };

  // =====================================================
  // START CAMERA
  // =====================================================

  const startCamera =
    async () => {
      try {
        if (
          !navigator
            .mediaDevices
            ?.getUserMedia
        ) {
          throw new Error(
            'Trình duyệt hiện tại không hỗ trợ truy cập camera.'
          );
        }

        stopCamera();

        const stream =
          await navigator
            .mediaDevices
            .getUserMedia({
              video: {
                facingMode: {
                  ideal:
                    'environment',
                },
              },

              audio:
                false,
            });

        streamRef.current =
          stream;

        if (
          videoRef.current
        ) {
          videoRef.current
            .srcObject =
            stream;

          await videoRef.current
            .play();
        }

        setCameraOn(
          true
        );

        if (
          'BarcodeDetector'
          in window
        ) {
          const detector =
            new window
              .BarcodeDetector({
                formats: [
                  'qr_code',
                ],
              });

          detectorTimerRef.current =
            setInterval(
              async () => {
                try {
                  if (
                    !videoRef.current
                    ||
                    busyRef.current
                  ) {
                    return;
                  }

                  const codes =
                    await detector
                      .detect(
                        videoRef.current
                      );

                  const value =
                    codes?.[0]
                      ?.rawValue;

                  if (value) {
                    await processCode(
                      value
                    );
                  }
                } catch {
                  // Bỏ qua lỗi từng frame.
                }
              },
              700
            );
        } else {
          setMessage({
            type:
              'info',

            text:
              'Camera đã mở nhưng trình duyệt chưa hỗ trợ BarcodeDetector. Bạn vẫn có thể nhập mã thủ công.',
          });
        }
      } catch (err) {
        stopCamera();

        setMessage({
          type:
            'danger',

          text:
            err?.message
            ||
            'Không thể mở camera.',
        });
      }
    };

  // =====================================================
  // CLEANUP
  // =====================================================

  useEffect(() => {
    mountedRef.current =
      true;

    return () => {
      mountedRef.current =
        false;

      stopCamera(
        false
      );
    };
  }, []);

  const handleManualSubmit =
    (e) => {
      e.preventDefault();

      processCode(
        manualCode
      );
    };

  return (
    <div>
      <h1 className="dashboard-page-title">
        Check-in bằng QR
      </h1>

      <p className="text-secondary mb-4">
        Quét mã QR trên lịch hẹn hoặc nhập mã QR / mã đặt lịch thủ công.
      </p>

      {message.text && (
        <Notification
          type={
            message.type
          }
          message={
            message.text
          }
          onClose={() =>
            setMessage({
              type: '',
              text: '',
            })
          }
        />
      )}

      <div className="row g-4">
        <div className="col-lg-7">
          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <div className="d-flex justify-content-between align-items-center mb-3">
                <h5 className="fw-bold mb-0">
                  Quét mã QR
                </h5>

                <span
                  className={`badge ${
                    cameraOn
                      ? 'bg-success'
                      : 'bg-secondary'
                  }`}
                >
                  {cameraOn
                    ? 'Camera đang bật'
                    : 'Camera đang tắt'}
                </span>
              </div>

              <div
                className="rounded-4 bg-dark overflow-hidden d-flex align-items-center justify-content-center"
                style={{
                  minHeight:
                    420,
                }}
              >
                <video
                  ref={
                    videoRef
                  }
                  muted
                  playsInline
                  style={{
                    width:
                      '100%',

                    maxHeight:
                      500,

                    objectFit:
                      'cover',

                    display:
                      cameraOn
                        ? 'block'
                        : 'none',
                  }}
                />

                {!cameraOn && (
                  <div className="text-center text-white-50 p-5">
                    <i className="fa-solid fa-qrcode fs-1 d-block mb-3" />

                    <div className="fw-semibold">
                      Camera chưa được bật
                    </div>

                    <div className="small mt-2">
                      Nhấn nút bên dưới để bắt đầu quét.
                    </div>
                  </div>
                )}
              </div>

              <div className="d-flex gap-2 mt-3">
                {!cameraOn ? (
                  <button
                    type="button"
                    className="btn btn-primary"
                    onClick={
                      startCamera
                    }
                    disabled={
                      processing
                    }
                  >
                    <i className="fa-solid fa-camera me-2" />
                    Bật camera
                  </button>
                ) : (
                  <button
                    type="button"
                    className="btn btn-outline-danger"
                    onClick={() =>
                      stopCamera()
                    }
                  >
                    <i className="fa-solid fa-camera-slash me-2" />
                    Tắt camera
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>

        <div className="col-lg-5">
          <div className="card border-0 shadow-sm rounded-4">
            <div className="card-body p-4">
              <h5 className="fw-bold">
                Nhập mã thủ công
              </h5>

              <p className="text-secondary small">
                Có thể nhập MaQR hoặc MaDatLich.
              </p>

              <form
                onSubmit={
                  handleManualSubmit
                }
              >
                <label className="form-label fw-semibold">
                  Mã QR / mã đặt lịch
                </label>

                <input
                  type="text"
                  className="form-control form-control-lg"
                  placeholder="Ví dụ: DLK-... / DLXN-... / QR-..."
                  value={
                    manualCode
                  }
                  onChange={(e) =>
                    setManualCode(
                      e.target.value
                    )
                  }
                  autoFocus
                />

                <button
                  type="submit"
                  className="btn btn-primary w-100 mt-3"
                  disabled={
                    processing ||
                    !manualCode.trim()
                  }
                >
                  <i className="fa-solid fa-check me-2" />

                  {processing
                    ? 'Đang check-in...'
                    : 'Xác nhận check-in'}
                </button>
              </form>

              <div className="alert alert-light border mt-4 mb-0">
                <strong>
                  Lưu ý:
                </strong>{' '}
                camera cần quyền truy cập từ trình duyệt. HTTPS hoặc localhost thường cần thiết để sử dụng camera.
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}