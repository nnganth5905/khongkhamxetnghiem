import React from 'react';

const PROCESS_STEPS = [
  {
    step: 1,
    title: 'Tiếp đón',
    items: [
      'Người bệnh đến quầy tiếp đón để đăng ký khám bệnh.',
      'Cung cấp thông tin cá nhân (họ tên, năm sinh, số điện thoại, địa chỉ).',
      'Nêu rõ lý do đến khám hoặc các triệu chứng đang gặp phải.',
      'Nhận số thứ tự khám bệnh.',
    ],
  },
  {
    step: 2,
    title:
      'Khám sàng lọc',
    items: [
      'Nhân viên y tế tiếp nhận, kiểm tra các chỉ số cơ bản như: huyết áp, nhịp tim, nhiệt độ, cân nặng, chiều cao.',
      'Ghi nhận triệu chứng ban đầu, hỏi thêm về tiền sử bệnh lý, dị ứng thuốc (nếu có).',
      'Tư vấn và hướng dẫn bệnh nhân thực hiện các bước tiếp theo (xét nghiệm, khám chuyên khoa…).',
    ],
  },
  {
    step: 3,
    title:
      'Xét nghiệm, chẩn đoán',
    items: [
      'Thực hiện các xét nghiệm cần thiết theo chỉ định: xét nghiệm máu, nước tiểu, chẩn đoán hình ảnh (X-quang, siêu âm, CT scan, MRI…).',
      'Kết quả xét nghiệm sẽ được lưu vào hồ sơ bệnh án và chuyển đến bác sĩ chuyên khoa.',
      'Nếu cần, bệnh nhân sẽ làm thêm các kiểm tra chuyên sâu khác.',
    ],
  },
  {
    step: 4,
    title:
      'Khám chuyên khoa',
    items: [
      'Bác sĩ chuyên khoa tiếp nhận hồ sơ bệnh án và kết quả xét nghiệm.',
      'Thăm khám lâm sàng, đối chiếu với kết quả cận lâm sàng để đưa ra chẩn đoán.',
      'Giải thích cho người bệnh về tình trạng sức khỏe, mức độ bệnh, phương án điều trị phù hợp.',
    ],
  },
  {
    step: 5,
    title: 'Điều trị',
    items: [
      'Bác sĩ tư vấn phác đồ điều trị.',
      'Kê đơn thuốc điều trị nội khoa (nếu cần).',
      'Với những bệnh lý đặc biệt, có thể chỉ định điều trị ngoại trú, nhập viện hoặc can thiệp phẫu thuật.',
      'Hướng dẫn người bệnh cách dùng thuốc, chế độ dinh dưỡng, nghỉ ngơi và theo dõi sức khỏe tại nhà.',
    ],
  },
  {
    step: 6,
    title: 'Tái khám',
    items: [
      'Người bệnh quay lại tái khám theo lịch hẹn hoặc khi có dấu hiệu bất thường.',
      'Bác sĩ đánh giá hiệu quả điều trị, điều chỉnh đơn thuốc hoặc phương pháp điều trị (nếu cần).',
      'Tiếp tục theo dõi lâu dài để đảm bảo sức khỏe ổn định.',
    ],
  },
];

export default function QuyTrinh() {
  return (
    <div className="medical-process-page bg-white">
      <section className="title-section text-center py-5">
        <div
          className="container"
          style={{
            maxWidth:
              '1200px',

            margin:
              '0 auto',

            padding:
              '0 20px',
          }}
        >
          <h3
            className="fw-bold mb-3 text-uppercase"
            style={{
              color:
                'var(--primary, #0b63e5)',

              letterSpacing:
                '1px',
            }}
          >
            QUY TRÌNH XÉT NGHIỆM,
            KHÁM CHỮA BỆNH
          </h3>

          <p
            className="text-secondary mx-auto mb-0"
            style={{
              maxWidth:
                '800px',

              lineHeight:
                '1.7',
            }}
          >
            Quy trình xét nghiệm, khám chữa bệnh
            như thế nào tại BIO MEDIC CENTER?
            Mời bạn đọc theo dõi bài viết dưới
            đây.
          </p>
        </div>
      </section>

      <section className="process-image text-center py-2">
        <div
          className="container"
          style={{
            maxWidth:
              '1200px',

            margin:
              '0 auto',

            padding:
              '0 20px',
          }}
        >
          <img
            src="https://phien123.github.io/webPK/quy_trinh.png"
            alt="Quy trình xét nghiệm, khám bệnh"
            className="img-fluid rounded-3 shadow-sm"
            style={{
              maxWidth:
                '65%',

              height:
                'auto',
            }}
          />
        </div>
      </section>

      <section className="process-steps py-5">
        <div
          className="container"
          style={{
            maxWidth:
              '1200px',

            margin:
              '0 auto',

            padding:
              '0 20px',
          }}
        >
          <h3
            className="section-title text-center fw-bold mb-5"
            style={{
              color:
                'var(--primary, #0b63e5)',

              fontSize:
                '2rem',
            }}
          >
            Chi tiết quy trình khám chữa bệnh
          </h3>

          <div className="steps-container d-flex flex-column gap-4">
            {PROCESS_STEPS.map(
              (item) => (
                <div
                  key={
                    item.step
                  }
                  className="step d-flex align-items-start p-4 bg-white rounded-3 shadow-sm border"
                  style={{
                    transition:
                      'transform 0.3s ease',
                  }}
                >
                  <div
                    className="step-number d-flex align-items-center justify-content-center text-white fw-bold rounded-circle flex-shrink-0 me-3"
                    style={{
                      backgroundColor:
                        'var(--primary, #0b63e5)',

                      width:
                        '40px',

                      height:
                        '40px',
                    }}
                  >
                    {
                      item.step
                    }
                  </div>

                  <div className="step-content">
                    <h4
                      className="fw-bold mb-2"
                      style={{
                        color:
                          'var(--primary, #0b63e5)',

                        fontSize:
                          '1.3rem',
                      }}
                    >
                      {
                        item.title
                      }
                    </h4>

                    <ul className="ps-3 mb-0">
                      {item.items.map(
                        (
                          sub,
                          idx
                        ) => (
                          <li
                            key={
                              idx
                            }
                            className="mb-2 text-secondary"
                            style={{
                              lineHeight:
                                '1.6',
                            }}
                          >
                            {
                              sub
                            }
                          </li>
                        )
                      )}
                    </ul>
                  </div>
                </div>
              )
            )}
          </div>
        </div>
      </section>
    </div>
  );
}