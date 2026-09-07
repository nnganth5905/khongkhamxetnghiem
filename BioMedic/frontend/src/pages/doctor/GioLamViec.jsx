import React from 'react';

export default function GioLamViec() {
  return (
    <div className="working-hours-page bg-white text-dark pb-5">
      <div
        className="header1 text-white text-center position-relative d-flex flex-column justify-content-center align-items-center"
        style={{
          background:
            "url('https://phien123.github.io/webPK/image.jpg') center/cover no-repeat",
          height: '300px',
        }}
      >
        <div
          className="overlay position-absolute"
          style={{
            inset: 0,
            backgroundColor:
              'rgba(0, 86, 179, 0.6)',
          }}
        />

        <h1
          className="fw-bold position-relative z-1 mb-2"
          style={{
            fontSize:
              'clamp(28px, 4vw, 50px)',
          }}
        >
          GIỜ LÀM VIỆC
        </h1>

        <p
          className="breadcrumb position-relative z-1 mb-0"
          style={{
            fontSize:
              'clamp(14px, 2vw, 20px)',
          }}
        >
          ĐẾN BIO MEDIC CENTER XÉT NGHIỆM
          THỨ 7, CHỦ NHẬT VÀ NGÀY LỄ
        </p>
      </div>

      <div
        className="container my-5"
        style={{
          maxWidth: '1200px',
        }}
      >
        <div
          className="table-wrapper mx-auto mb-5 rounded-3 overflow-hidden shadow-sm"
          style={{
            maxWidth: '600px',
            border:
              '2px solid var(--primary, #0b63e5)',
          }}
        >
          <table
            className="table table-bordered text-center m-0 align-middle"
            style={{
              fontSize: '18px',
            }}
          >
            <thead>
              <tr
                style={{
                  backgroundColor:
                    'var(--primary, #0b63e5)',
                  color: '#fff',
                }}
              >
                <th
                  className="py-3 text-white"
                  style={{
                    backgroundColor:
                      'var(--primary, #0b63e5)',
                  }}
                >
                  Thứ
                </th>

                <th
                  className="py-3 text-white"
                  style={{
                    backgroundColor:
                      'var(--primary, #0b63e5)',
                  }}
                >
                  Giờ làm việc
                </th>
              </tr>
            </thead>

            <tbody>
              <tr>
                <td className="py-3">
                  Thứ hai - Thứ sáu
                </td>

                <td className="py-3 fw-semibold">
                  7:00 - 19:00
                </td>
              </tr>

              <tr>
                <td className="py-3">
                  Thứ bảy
                </td>

                <td className="py-3 fw-semibold">
                  7:00 - 16:00
                </td>
              </tr>

              <tr>
                <td className="py-3">
                  Chủ nhật
                </td>

                <td className="py-3 fw-semibold">
                  7:00 - 16:00
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div className="row g-4">
          <div
            className="col-lg-8 text-secondary"
            style={{
              textAlign:
                'justify',
              lineHeight:
                '1.8',
            }}
          >
            <h2
              className="fw-bold mb-4 pb-2"
              style={{
                color:
                  'var(--primary, #0b63e5)',

                fontSize:
                  '24px',

                borderBottom:
                  '2px solid var(--primary, #0b63e5)',
              }}
            >
              ĐẾN BIO MEDIC CENTER KHÁM BỆNH
              THỨ 7, CHỦ NHẬT VÀ NGÀY LỄ
            </h2>

            <p className="text-dark">
              Nhằm bảo đảm công tác phục vụ chăm
              sóc sức khỏe cộng đồng không bị
              gián đoạn, Bio Medic Center tổ chức
              xét nghiệm đầy đủ, cũng như cung cấp
              các dịch vụ y tế tiện ích vào ngày
              nghỉ cuối tuần và ngày lễ tết với
              thủ tục nhanh gọn, chất lượng nhưng
              giá dịch vụ vẫn thu đúng giá niêm
              yết.
            </p>

            <h4
              className="fw-bold text-dark mt-4 mb-3"
              style={{
                fontSize:
                  '18px',
              }}
            >
              Xét nghiệm ngày nghỉ - Sự cần thiết
              để phát hiện, chăm sóc sức khỏe kịp
              thời
            </h4>

            <p>
              Phần lớn người dân nước ta có thói
              quen đi khám chữa bệnh giờ hành
              chính, vì vậy, người làm hành chính
              như học sinh, sinh viên, hay người eo
              hẹp về thời gian nhưng bị bệnh lý mạn
              tính,… thì việc đi khám như hiện nay
              đã gây nên không ít rào cản, khó khăn
              với người bệnh.
            </p>

            <p>
              Thêm nữa, những bất thường về sức
              khỏe như rối loạn tiêu hóa, đau bụng,
              đau đầu,… có thể xảy ra bất cứ lúc nào
              nên nhu cầu đến cơ sở y tế để được
              thăm khám và điều trị kịp thời là rất
              cần thiết. Bởi vậy, chủ động đi khám
              ngày nghỉ, khám ngoài giờ giúp giảm
              những phiền hà, e ngại và thay vào đó
              là sự an tâm đáp ứng kịp thời nhu cầu
              kiểm tra sức khỏe của người dân.
            </p>

            <p>
              Với mong muốn mang đến khách hàng sự
              thuận lợi, kịp thời xét nghiệm và tư
              vấn, bên cạnh thời gian làm việc tất
              cả các ngày trong tuần, Bio Medic
              Center còn tổ chức xét nghiệm và tư
              vấn vào ngày nghỉ cuối tuần, các ngày
              lễ, ngày Tết.
            </p>

            <div
              className="p-3 my-4 rounded-end"
              style={{
                backgroundColor:
                  '#e7f1ff',

                borderLeft:
                  '4px solid var(--primary, #0b63e5)',
              }}
            >
              <p className="fw-bold mb-2 text-dark">
                BIO MEDIC CENTER PHỤC VỤ
              </p>

              <ul className="mb-0 ps-3">
                <li>
                  Xét nghiệm: ADN, Máu, Nước tiểu,
                  Sinh học Phân Tử, ...
                </li>

                <li>
                  Thời gian làm việc hành chính:

                  <ul>
                    <li>
                      Sáng: từ 7h00 đến 12h00
                    </li>

                    <li>
                      Chiều: từ 13h30 đến 19h00
                    </li>
                  </ul>
                </li>
              </ul>
            </div>

            <p>
              Chia sẻ về hoạt động xét nghiệm ngày
              nghỉ, Thạc sỹ Hoàng Thị Thúy - Phó
              Giám đốc Bio Medic Center cho biết:
              "Bệnh viện hiện có đội ngũ nhân sự
              hùng hậu với cán bộ y bác sĩ, trong
              đó có cả chuyên gia đầu ngành đã từng
              công tác, học tập ở các bệnh viện
              danh tiếng trong nước và quốc tế tham
              gia tư vấn chuyên môn. Theo đó, ngoài
              việc bố trí xét nghiệm ngày hành
              chính, Bio Medic Center bảo đảm có
              đầy đủ đội ngũ bác sĩ, chuyên gia làm
              việc thứ 7 và chủ nhật hàng tuần."
            </p>

            <p>
              Không chỉ tạo sự thuận lợi dễ dàng
              sắp xếp thời gian, Bio Medic Center
              cam kết chất lượng chuyên môn chính
              xác, bởi những thế mạnh về hệ thống
              máy móc hiện đại của các chuyên khoa,…
            </p>

            <p>
              Với phương châm: "Bio Medic Center -
              Dịch vụ tốt, Công nghệ cao", Bio Medic
              Center còn mang đến sự hài lòng hơn
              cho khách hàng về thủ tục hành chính
              nhanh gọn, phí xét nghiệm, phí dịch
              vụ y tế vẫn thu đúng giá niêm yết như
              ngày thường.
            </p>
          </div>

          <div className="col-lg-4">
            <div
              className="p-4 rounded-3 shadow-sm text-center"
              style={{
                backgroundColor:
                  '#f0f8ff',
              }}
            >
              <h3
                className="fw-bold fs-5 mb-2"
                style={{
                  color:
                    'var(--primary, #0b63e5)',
                }}
              >
                BIO MEDIC CENTER
              </h3>

              <h3
                className="fw-bold fs-6 mb-3"
                style={{
                  color:
                    'var(--primary, #0b63e5)',
                }}
              >
                SỐNG KHOẺ TRONG TẦM TAY
              </h3>

              <a
                href="tel:1900565656"
                className="btn btn-primary d-inline-flex align-items-center gap-2 px-4 py-2 fw-bold fs-5 rounded-3 mb-3 text-decoration-none text-white shadow-sm"
                style={{
                  backgroundColor:
                    'var(--primary, #0b63e5)',

                  borderColor:
                    'var(--primary, #0b63e5)',
                }}
                aria-label="Gọi 1900 56 56 56"
              >
                <i className="fa-solid fa-phone" />{' '}
                1900 56 56 56
              </a>

              <div className="my-3">
                <img
                  src="https://phien123.github.io/webPK/1.png"
                  alt="Hình ảnh Bio Medic Center"
                  className="img-fluid rounded-3 shadow-sm w-100"
                  style={{
                    maxHeight:
                      '300px',

                    objectFit:
                      'cover',
                  }}
                />
              </div>

              <div className="text-start mt-4">
                <h4
                  className="fw-bold fs-6 mb-3"
                  style={{
                    color:
                      'var(--primary, #0b63e5)',
                  }}
                >
                  Dịch vụ nổi bật
                </h4>

                <ul className="list-unstyled mb-0">
                  {[
                    'Xét nghiệm ADN',
                    'Xét nghiệm Máu',
                    'Xét nghiệm Hormone',
                    'Xét nghiệm Dị ứng',
                    'Xét nghiệm Nước tiểu',
                  ].map(
                    (
                      srv,
                      idx
                    ) => (
                      <li
                        key={
                          idx
                        }
                        className="py-2 border-bottom border-dashed text-secondary"
                      >
                        <span className="text-primary fw-bold me-2">
                          ✓
                        </span>

                        {srv}
                      </li>
                    )
                  )}
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}