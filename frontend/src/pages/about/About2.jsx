import React from 'react';

const FACILITIES_DATA = [
  {
    id: 1,
    title: '1. Xét nghiệm nước tiểu',
    image:
      'https://choyte.com/images/2015/03/23/cobas-e-411.gif',
    caption:
      'Máy Roche Cobas cho kết quả nhanh và chính xác',
    equipments: [
      {
        name: 'Máy phân tích nước tiểu tự động',
        desc:
          'Sysmex UF-1000i cung cấp tổng phân tích tế bào và vi trùng nước tiểu (TBCL/TBVT) với độ chính xác cao.',
      },
      {
        name: 'Máy hóa sinh',
        desc:
          'Roche Cobas đo Microalbumin, Ure, Creatinine trong nước tiểu.',
      },
      {
        name: 'Kính hiển vi',
        desc:
          'Hỗ trợ phân tích vi thể trầm tích nước tiểu.',
      },
    ],
  },

  {
    id: 2,
    title: '2. Xét nghiệm bệnh lý thận',
    image:
      'https://anmed.com.vn/wp-content/uploads/2023/09/may-phan-tich-dien-giai-3-thong-so-bm-ise.jpg',
    caption:
      'Máy xét nghiệm điện giải (Na⁺, K⁺, Cl⁻, Ca²⁺, pH)',
    equipments: [
      {
        name: 'Máy phân tích hóa sinh tự động',
        desc:
          'Beckman Coulter và Roche Cobas đo Albumin, Ure, Creatinine, Cystatin C.',
      },
      {
        name: 'Máy đo điện giải (ISE)',
        desc:
          'Phân tích Na⁺, K⁺, Cl⁻ đánh giá rối loạn điện giải liên quan chức năng thận.',
      },
      {
        name: 'Máy điện di protein',
        desc:
          'Sebia Capillarys phân tích protein huyết thanh và nước tiểu.',
      },
    ],
  },

  {
    id: 3,
    title: '3. Xét nghiệm tiêu hóa – gan mật – tụy',
    image:
      'https://medigroupasia.com/wp-content/uploads/2021/01/fibroscan-530-compact-with-roll-stand.png',
    caption: 'Máy FibroScan',
    equipments: [
      {
        name: 'Máy phân tích hóa sinh',
        desc:
          'Đo ALT, AST, GGT, ALP và Bilirubin (trực tiếp/gián tiếp).',
      },
      {
        name: 'Máy miễn dịch tự động',
        desc:
          'Phát hiện marker viêm gan (HBsAg, Anti-HCV) và tải lượng (HBV-DNA, HCV-RNA).',
      },
      {
        name: 'Máy FibroScan',
        desc:
          'Đánh giá độ xơ hóa gan không xâm lấn, hỗ trợ chẩn đoán xơ gan và gan nhiễm mỡ.',
      },
    ],
  },

  {
    id: 4,
    title: '4. Xét nghiệm nhiễm virus – vi khuẩn',
    image:
      'https://minhkhangmedical.com/wp-content/uploads/2020/04/Abbott-Architec-i1000sr.jpg',
    caption: 'Hệ thống Abbott ARCHITECT i1000SR',
    equipments: [
      {
        name: 'Máy miễn dịch tự động',
        desc:
          'Phát hiện kháng thể/kháng nguyên sởi, quai bị, thủy đậu, Dengue (IgM/IgG, NS1) và giang mai (RPR).',
      },
      {
        name: 'Tủ cấy an toàn sinh học cấp II',
        desc:
          'Đảm bảo an toàn khi cấy máu/dịch và làm kháng sinh đồ.',
      },
      {
        name: 'Kính hiển vi huỳnh quang',
        desc:
          'Hỗ trợ chẩn đoán nhanh sốt rét và các bệnh nhiễm trùng khác.',
      },
      {
        name: 'Test nhanh',
        desc:
          'Kết quả tức thì cho cúm A/B, Dengue…',
      },
    ],
  },

  {
    id: 5,
    title: '5. Xét nghiệm cao cấp',
    image:
      'https://biomedic.com.vn/wp-content/uploads/2023/09/COL32204-QuantStudio-5-Right-Trans-5inWht-2048x2048-1-e1694486870145.jpeg',
    caption: 'Máy PCR thời gian thực',
    equipments: [
      {
        name: 'Real-time PCR',
        desc:
          'Roche LightCycler, Thermo Fisher QuantStudio phục vụ NIPT, gen Thalassemia, ADN huyết thống, sàng lọc gen ung thư.',
      },
      {
        name: 'Giải trình tự gen (NGS)',
        desc:
          'Illumina MiSeq/NextSeq hỗ trợ phân tích di truyền chi tiết, bao gồm HLA lớp I/II.',
      },
      {
        name: 'Phân tích nhiễm sắc thể',
        desc:
          'Kính hiển vi huỳnh quang và phần mềm Metafer phát hiện bất thường di truyền.',
      },
      {
        name: 'ELISA chuyên sâu',
        desc:
          'Định lượng kháng thể acetylcholine receptor (AChR) hỗ trợ chẩn đoán nhược cơ.',
      },
    ],
  },

  {
    id: 6,
    title: '6. Xét nghiệm ung bướu',
    image:
      'https://mediexpress.com.vn/upload/news/3040ef50c15a05045c4b-4155.jpg',
    caption:
      'Máy xét nghiệm miễn dịch tự động eCL8000',
    equipments: [
      {
        name: 'Máy miễn dịch tự động',
        desc:
          'Định lượng marker ung thư CEA, AFP, CA 125, CA 19-9, PSA…',
      },
      {
        name: 'PCR và NGS',
        desc:
          'Phát hiện đột biến gen liên quan ung thư (ví dụ BRCA, EGFR).',
      },
      {
        name: 'Sinh thiết lỏng',
        desc:
          'Phát hiện DNA khối u lưu hành (liquid biopsy).',
      },
    ],
  },

  {
    id: 7,
    title: '7. Xét nghiệm huyết học',
    image:
      'https://vannienco.vn/Uploads/images/products/esr300.jpg',
    caption: 'Máy đo tốc độ lắng máu (ESR)',
    equipments: [
      {
        name: 'Máy phân tích huyết học',
        desc:
          'Sysmex XN-Series cho tổng phân tích tế bào máu (CBC) và các chỉ số mở rộng.',
      },
      {
        name: 'Kính hiển vi chuyên dụng',
        desc:
          'Đọc tiêu bản máu ngoại vi và đánh giá hình thái.',
      },
      {
        name: 'Máy đo tốc độ lắng máu (ESR)',
        desc:
          'Hỗ trợ đánh giá viêm.',
      },
    ],
  },

  {
    id: 8,
    title: '8. Xét nghiệm đông máu',
    image:
      'https://hoachatxetnghiem.com.vn/Pictures/Dutch%20DCA-1.jpg',
    caption: 'Máy phân tích đông máu',
    equipments: [
      {
        name: 'Máy đông máu tự động',
        desc:
          'Sysmex CS-Series đo PT, APTT, INR, D-dimer.',
      },
      {
        name: 'Định lượng yếu tố đông máu',
        desc:
          'Fibrinogen và các yếu tố đặc hiệu khác.',
      },
      {
        name: 'Panel đông máu chuyên sâu',
        desc:
          'Hỗ trợ chẩn đoán rối loạn đông máu.',
      },
    ],
  },

  {
    id: 9,
    title: '9. Xét nghiệm nội tiết',
    image:
      'https://vietnguyenco.vn/wp-content/uploads/2021/03/hplc-scion-lc6000.png',
    caption: 'Máy HPLC',
    equipments: [
      {
        name: 'Máy miễn dịch tự động',
        desc:
          'Roche Elecsys đo hormone TSH, FT3, FT4, Cortisol, Insulin…',
      },
      {
        name: 'HPLC',
        desc:
          'Phân tích catecholamine và metanephrine trong nghi ngờ u tủy thượng thận.',
      },
      {
        name: 'Máy đo HbA1c',
        desc:
          'Đánh giá kiểm soát đường huyết dài hạn.',
      },
    ],
  },

  {
    id: 10,
    title: '10. Xét nghiệm lipid máu',
    image:
      'https://innolab-ks.com/wp-content/uploads/2024/05/4304-1.jpg',
    caption: 'Máy phân tích hóa sinh',
    equipments: [
      {
        name: 'Máy hóa sinh',
        desc:
          'Định lượng TC, TG, HDL-C, LDL-C, non-HDL-C.',
      },
      {
        name: 'Điện di lipoprotein',
        desc:
          'Hỗ trợ phân tích rối loạn lipid máu phức tạp.',
      },
      {
        name: 'Định lượng ApoA1/ApoB',
        desc:
          'Đánh giá nguy cơ tim mạch sâu hơn.',
      },
    ],
  },
];

export default function About2() {
  return (
    <div className="facilities-page bg-light py-4">
      <div
        className="container"
        style={{ maxWidth: '1200px' }}
      >
        <h2 className="text-center my-4 fw-bold fs-2">
          <b style={{ color: 'var(--primary)' }}>
            Cơ sở vật chất
          </b>
        </h2>

        {/* Giới thiệu */}
        <div className="mb-4">
          <h3
            className="fw-bold fs-4 mb-3"
            style={{ color: 'var(--primary)' }}
          >
            Giới thiệu
          </h3>

          <div className="card border-0 shadow-sm p-4 rounded-3">
            <p
              className="mb-0"
              style={{ lineHeight: '1.7' }}
            >
              Chào mừng quý khách đến với Bio Medic Center,
              phòng khám xét nghiệm y khoa tiên tiến với hệ
              thống trang thiết bị hiện đại, đáp ứng nhu cầu
              chẩn đoán từ cơ bản đến chuyên sâu. Chúng tôi sở
              hữu cơ sở vật chất đạt chuẩn quốc tế, đảm bảo kết
              quả xét nghiệm{' '}
              <b style={{ color: 'var(--primary)' }}>
                chính xác – nhanh chóng – an toàn
              </b>{' '}
              cho các nhóm: bệnh lý thận – tiết niệu, nhiễm
              virus – vi khuẩn, xét nghiệm cao cấp, tiêu hóa –
              gan mật – tụy, huyết học, đông máu, nội tiết và
              lipid máu.
            </p>
          </div>
        </div>

        {/* Danh sách 10 mục trang thiết bị */}
        <section>
          {FACILITIES_DATA.map((item) => (
            <div
              className="mb-4"
              key={item.id}
            >
              <h3
                className="fw-bold fs-4 mb-3"
                style={{ color: 'var(--primary)' }}
              >
                {item.title}
              </h3>

              <div className="row g-3 align-items-stretch">
                <div className="col-md-6">
                  <div className="card border-0 shadow-sm p-3 h-100 rounded-3">
                    <div
                      className="d-flex align-items-center justify-content-center bg-light rounded-3 p-2 h-100 overflow-hidden"
                      style={{ minHeight: '220px' }}
                    >
                      <img
                        src={item.image}
                        alt={item.caption}
                        className="img-fluid rounded-3"
                        style={{
                          maxHeight: '250px',
                          objectFit: 'contain',
                        }}
                      />
                    </div>

                    <p className="text-center text-muted small fst-italic mt-2 mb-0">
                      {item.caption}
                    </p>
                  </div>
                </div>

                <div className="col-md-6">
                  <div className="card border-0 shadow-sm p-4 h-100 rounded-3">
                    <h4
                      className="text-center mb-3 fw-bold fs-5"
                      style={{ color: 'var(--primary)' }}
                    >
                      Thiết bị hiện đại
                    </h4>

                    <ul className="list-unstyled mb-0">
                      {item.equipments.map((eq, idx) => (
                        <li
                          key={idx}
                          className="mb-3 position-relative ps-4"
                          style={{ lineHeight: '1.6' }}
                        >
                          <span
                            className="position-absolute start-0 fw-bold"
                            style={{
                              color: 'var(--primary)',
                            }}
                          >
                            •
                          </span>

                          <strong
                            style={{
                              color: 'var(--primary)',
                            }}
                          >
                            {eq.name}
                          </strong>
                          : {eq.desc}
                        </li>
                      ))}
                    </ul>
                  </div>
                </div>
              </div>
            </div>
          ))}
        </section>

        {/* Cam kết chất lượng */}
        <div className="mt-4 pb-4">
          <h3
            className="fw-bold fs-4 mb-3"
            style={{ color: 'var(--primary)' }}
          >
            Cam kết chất lượng
          </h3>

          <div
            className="card border-0 shadow-sm p-4 rounded-3"
            style={{ lineHeight: '1.7' }}
          >
            <p className="mb-2">
              <b style={{ color: 'var(--primary)' }}>
                Phòng sạch đạt chuẩn
              </b>
              : Đáp ứng ISO 5 và BSL-2, đảm bảo an toàn sinh
              học và giảm nhiễm bẩn mẫu.
            </p>

            <p className="mb-2">
              <b style={{ color: 'var(--primary)' }}>
                Hệ thống quản lý thông tin xét nghiệm (LIS)
              </b>
              : Tăng độ chính xác, bảo mật và truy xuất dữ liệu.
            </p>

            <p className="mb-2">
              <b style={{ color: 'var(--primary)' }}>
                Đội ngũ chuyên gia
              </b>
              : Đào tạo chuyên sâu về hóa sinh, vi sinh, sinh
              học phân tử và di truyền học.
            </p>

            <p className="mb-2">
              <b style={{ color: 'var(--primary)' }}>
                Quy trình quốc tế
              </b>
              : Tuân thủ ISO 15189 và CAP, mang lại kết quả đáng
              tin cậy.
            </p>

            <p className="mb-0">
              Hãy trải nghiệm dịch vụ xét nghiệm y khoa chất
              lượng cao tại Bio Medic Center. Liên hệ ngay để
              được tư vấn và đặt lịch!
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}