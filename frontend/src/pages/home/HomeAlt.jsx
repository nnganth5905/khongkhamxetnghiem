import React, {
  useEffect,
  useRef,
  useState,
} from 'react';

import { Link } from 'react-router-dom';

import {
  getHomeData,
  submitConsultation,
} from '../../services/homeService';

const INITIAL_DOCTORS = [
  {
    id: 1,
    name: 'BS. Nguyễn Văn A',
    title:
      'Bác sĩ Chuyên khoa II',
    image:
      'https://placehold.co/240x240?text=Doctor+1',
  },
  {
    id: 2,
    name: 'ThS. Trần Thị B',
    title:
      'Thạc sĩ - Bác sĩ Xét nghiệm',
    image:
      'https://placehold.co/240x240?text=Doctor+2',
  },
  {
    id: 3,
    name: 'BS. Lê Văn C',
    title:
      'Bác sĩ Đa khoa',
    image:
      'https://placehold.co/240x240?text=Doctor+3',
  },
  {
    id: 4,
    name: 'TS. Phạm Minh D',
    title:
      'Tiến sĩ Y khoa',
    image:
      'https://placehold.co/240x240?text=Doctor+4',
  },
];

const INITIAL_SERVICES = [
  {
    id: 1,
    title:
      'Xét nghiệm ADN',
    desc:
      'Trong 1 ngày có kết quả',
    priceNow:
      '999,000đ',
    priceOld:
      '1,299,000đ',
    image:
      'https://www.vinmec.com/static/uploads/20190822_085457_758016_DNA_test_max_1800x1800_jpg_5dc92dd486.jpg',
  },
  {
    id: 2,
    title:
      'Xét nghiệm Máu',
    desc:
      'Xét nghiệm công thức máu toàn phần',
    priceNow:
      '299,000đ',
    priceOld:
      '450,000đ',
    image:
      'https://medlatec.vn/media/14520/content/20200722_xet-nghiem-mau-01.jpg',
  },
  {
    id: 3,
    title:
      'Xét nghiệm Nước Tiểu',
    desc:
      'Phát hiện các bệnh về thận và đường tiết niệu',
    priceNow:
      '199,000đ',
    priceOld:
      '300,000đ',
    image:
      'https://umcclinic.com.vn/Data/Sites/1/media/y-hoc-thuong-thuc/x%C3%A9t-nghi%E1%BB%87m/x%C3%A9t-nghi%E1%BB%87m-t%E1%BB%95ng-ph%C3%A2n-t%C3%ADch-n%C6%B0%E1%BB%9Bc-ti%E1%BB%83u/x%C3%A9t-nghi%E1%BB%87m-t%E1%BB%95ng-ph%C3%A2n-t%C3%ADch-n%C6%B0%E1%BB%9Bc-ti%E1%BB%83u-l%C3%A0-g%C3%AC/xet-nghiem-nuoc-tieu-danh-gia-chuc-nang-than.jpg',
  },
  {
    id: 4,
    title:
      'Xét nghiệm Sinh Học Phân Tử',
    desc:
      'PCR, giải trình tự gen',
    priceNow:
      '2,500,000đ',
    priceOld:
      '3,200,000đ',
    image:
      'https://www.vinmec.com/static/uploads/20190822_085237_833968_xet_nghiem_gen_max_1800x1800_jpg_2ad6d792c1.jpg',
  },
  {
    id: 5,
    title:
      'Xét nghiệm Dị Ứng',
    desc:
      'Xác định các tác nhân gây dị ứng',
    priceNow:
      '1,200,000đ',
    priceOld:
      '1,500,000đ',
    image:
      'https://tamanhhospital.vn/wp-content/uploads/2022/01/chuan-bi-truoc-khi-xet-nghiem.jpg',
  },
  {
    id: 6,
    title:
      'Xét nghiệm Hormone',
    desc:
      'Kiểm tra nội tiết tố toàn diện',
    priceNow:
      '1,800,000đ',
    priceOld:
      '2,200,000đ',
    image:
      'https://benhvienphuongdong.vn/public/uploads/2024/thang-10/xet-nghiem-hormone-tuyen-giap/xet-nghiem-hormone-tuyen-giap-1.jpg',
  },
];

const INITIAL_NEWS = [
  {
    id: 1,
    title:
      'Thổi bay u dưới niêm mạc không cần phẫu thuật',
    content:
      'Xuất hiện khối u dưới niêm mạc… điều trị thành công bằng phương pháp nội soi tiên tiến.',
    image:
      'https://placehold.co/400x300/0B63E5/FFFFFF/png?text=Tin+T%E1%BB%A9c+Y+Khoa',
  },
  {
    id: 2,
    title:
      'Phương pháp mới điều trị ung thư',
    content:
      'Các nhà khoa học vừa công bố phương pháp điều trị ung thư mới với tỷ lệ thành công cao.',
    image:
      'https://placehold.co/400x300/0B63E5/FFFFFF/png?text=Tin+T%E1%BB%A9c+Y+Khoa',
  },
  {
    id: 3,
    title:
      'Cách phòng ngừa bệnh tim mạch',
    content:
      'Những thói quen sống lành mạnh giúp giảm nguy cơ mắc các bệnh tim mạch hiệu quả.',
    image:
      'https://placehold.co/400x300/0B63E5/FFFFFF/png?text=Tin+T%E1%BB%A9c+Y+Khoa',
  },
];

const normalizeDoctors = (items = []) =>
  items.map((d) => ({
    id:
      d.id ??
      d.IDBacSi,

    name:
      d.name ??
      d.TenBacSi,

    title:
      d.title ??
      d.ChucDanh ??
      '',

    image:
      d.image ??
      d.HinhAnh ??
      'https://placehold.co/240x240?text=Doctor',
  }));

const normalizeServices = (items = []) =>
  items.map((item) => ({
    id:
      item.id ??
      item.IDXetNghiem,

    title:
      item.title ??
      item.TenXetNghiem,

    desc:
      item.desc ??
      item.MoTa ??
      '',

    priceNow:
      item.priceNow ??
      item.GiaHienTai ??
      (item.Gia != null
        ? `${Number(
            item.Gia
          ).toLocaleString(
            'vi-VN'
          )}đ`
        : ''),

    priceOld:
      item.priceOld ??
      item.GiaCu ??
      '',

    image:
      item.image ??
      item.HinhAnh ??
      'https://placehold.co/400x300?text=Service',
  }));

const normalizeNews = (items = []) =>
  items.map((item) => ({
    id:
      item.id ??
      item.IDTinTuc,

    title:
      item.title ??
      item.TieuDe,

    content:
      item.content ??
      item.NoiDungNgan ??
      item.MoTa ??
      '',

    image:
      item.image ??
      item.HinhAnh ??
      'https://placehold.co/400x300/0B63E5/FFFFFF/png?text=Tin+T%E1%BB%A9c+Y+Khoa',
  }));

export default function HomeAlt({
  doctors: doctorsProp = null,
  services: servicesProp = null,
  news: newsProp = null,
}) {
  const trackRef =
    useRef(null);

  const [doctors, setDoctors] =
    useState(
      doctorsProp ||
        INITIAL_DOCTORS
    );

  const [services, setServices] =
    useState(
      servicesProp ||
        INITIAL_SERVICES
    );

  const [news, setNews] =
    useState(
      newsProp ||
        INITIAL_NEWS
    );

  const [formData, setFormData] =
    useState({
      fullName: '',
      phone: '',
      serviceId: '',
    });

  useEffect(() => {
    let active = true;

    const loadData =
      async () => {
        try {
          const data =
            await getHomeData();

          if (!active) {
            return;
          }

          if (
            !doctorsProp &&
            data?.doctors?.length
          ) {
            setDoctors(
              normalizeDoctors(
                data.doctors
              )
            );
          }

          if (
            !servicesProp &&
            data?.services?.length
          ) {
            setServices(
              normalizeServices(
                data.services
              )
            );
          }

          if (
            !newsProp &&
            data?.news?.length
          ) {
            setNews(
              normalizeNews(
                data.news
              )
            );
          }
        } catch {
          // Backend Home chưa có:
          // tiếp tục dùng dữ liệu cũ,
          // giao diện không bị mất.
        }
      };

    loadData();

    return () => {
      active = false;
    };
  }, [
    doctorsProp,
    servicesProp,
    newsProp,
  ]);

  const handleScroll = (
    direction
  ) => {
    if (
      trackRef.current
    ) {
      const card =
        trackRef.current.querySelector(
          '.team-card'
        );

      const step = card
        ? card.getBoundingClientRect()
            .width + 24
        : 320;

      trackRef.current.scrollBy({
        left:
          direction === 'left'
            ? -step
            : step,

        behavior:
          'smooth',
      });
    }
  };

  const handleInputChange =
    (e) => {
      const {
        name,
        value,
      } = e.target;

      setFormData(
        (prev) => ({
          ...prev,
          [name]: value,
        })
      );
    };

  const handleSubmit =
    async (e) => {
      e.preventDefault();

      try {
        await submitConsultation(
          formData
        );

        window.alert(
          'Cảm ơn bạn! Chúng tôi sẽ liên hệ trong thời gian sớm nhất.'
        );

        setFormData({
          fullName: '',
          phone: '',
          serviceId: '',
        });
      } catch {
        window.alert(
          'Cảm ơn bạn! Chúng tôi đã ghi nhận thông tin.'
        );
      }
    };

  return (
    <div className="bmc-wrapper">
      <section className="hero">
        <div className="container-fluid px-0">
          <div
            id="heroCarouselAlt"
            className="carousel slide carousel-fade bmc-hero"
            data-bs-ride="carousel"
            data-bs-interval="4200"
            data-bs-pause="hover"
          >
            <div className="carousel-indicators">
              <button
                type="button"
                data-bs-target="#heroCarouselAlt"
                data-bs-slide-to="0"
                className="active"
                aria-current="true"
                aria-label="Slide 1"
              />

              <button
                type="button"
                data-bs-target="#heroCarouselAlt"
                data-bs-slide-to="1"
                aria-label="Slide 2"
              />

              <button
                type="button"
                data-bs-target="#heroCarouselAlt"
                data-bs-slide-to="2"
                aria-label="Slide 3"
              />

              <button
                type="button"
                data-bs-target="#heroCarouselAlt"
                data-bs-slide-to="3"
                aria-label="Slide 4"
              />
            </div>

            <div className="carousel-inner">
              <div className="carousel-item active">
                <img
                  src="https://nganngan5905-tech.github.io/LTWEB/draft%201.png"
                  alt="Banner 1"
                  className="w-100 h-100 object-cover"
                />
              </div>

              <div className="carousel-item">
                <img
                  src="https://phien123.github.io/webPK/Banner2.png"
                  alt="Banner 2"
                  className="w-100 h-100 object-cover"
                />
              </div>

              <div className="carousel-item">
                <img
                  src="https://nganngan5905-tech.github.io/LTWEB/draft.png"
                  alt="Banner 3"
                  className="w-100 h-100 object-cover"
                />
              </div>

              <div className="carousel-item">
                <img
                  src="https://nganngan5905-tech.github.io/LTWEB/snapedit_1758634967670.jpeg"
                  alt="Banner 4"
                  className="w-100 h-100 object-cover"
                />
              </div>
            </div>

            <button
              className="carousel-control-prev"
              type="button"
              data-bs-target="#heroCarouselAlt"
              data-bs-slide="prev"
            >
              <span
                className="carousel-control-prev-icon"
                aria-hidden="true"
              />

              <span className="visually-hidden">
                Trước
              </span>
            </button>

            <button
              className="carousel-control-next"
              type="button"
              data-bs-target="#heroCarouselAlt"
              data-bs-slide="next"
            >
              <span
                className="carousel-control-next-icon"
                aria-hidden="true"
              />

              <span className="visually-hidden">
                Sau
              </span>
            </button>
          </div>
        </div>
      </section>

      <section className="features py-5">
        <div className="container">
          <h2 className="section-title">
            Tiện ích dành cho khách hàng
          </h2>

          <p className="section-subtitle">
            Các dịch vụ tiện ích hiện đại giúp khách
            hàng dễ dàng tiếp cận và sử dụng dịch vụ
            y tế chất lượng cao
          </p>

          <div className="row g-4 mt-2">
            <div className="col-md-6 col-lg-3">
              <Link
                to="/dat-lich-xet-nghiem"
                className="feature-card d-block text-decoration-none text-reset"
              >
                <div className="icon">
                  <i className="fa-solid fa-calendar-days" />
                </div>

                <h5>
                  Đặt lịch xét nghiệm
                </h5>

                <p>
                  Đặt lịch lấy mẫu tại cơ sở của Bio
                </p>
              </Link>
            </div>

            <div className="col-md-6 col-lg-3">
              <Link
                to="/tra-cuu-kqua"
                className="feature-card d-block text-decoration-none text-reset"
              >
                <div className="icon">
                  <i className="fa-solid fa-magnifying-glass" />
                </div>

                <h5>
                  Tra cứu kết quả
                </h5>

                <p>
                  Tra cứu kết quả dịch vụ y tế tại Hệ
                  thống Y tế Bio
                </p>
              </Link>
            </div>

            <div className="col-md-6 col-lg-3">
              <Link
                to="/bang-gia"
                className="feature-card d-block text-decoration-none text-reset"
              >
                <div className="icon">
                  <i className="fa-solid fa-stamp" />
                </div>

                <h5>
                  Bảng giá dịch vụ
                </h5>

                <p>
                  Tra cứu giá dịch vụ y tế tại Bio
                </p>
              </Link>
            </div>

            <div className="col-md-6 col-lg-3">
              <Link
                to="/tu-van"
                className="feature-card d-block text-decoration-none text-reset"
              >
                <div className="icon">
                  <i className="fa-solid fa-message" />
                </div>

                <h5>
                  Tư vấn đặt lịch
                </h5>

                <p>
                  Đặt câu hỏi, nhận lịch tự động
                </p>
              </Link>
            </div>
          </div>
        </div>
      </section>

      <section
        id="team"
        className="py-5 bg-light"
      >
        <div className="container">
          <h2 className="section-title">
            Đội ngũ của chúng tôi
          </h2>

          <p className="section-subtitle">
            Đội ngũ bác sĩ, chuyên gia đầu ngành với
            nhiều năm kinh nghiệm trong lĩnh vực y tế
          </p>

          <div className="team-viewport position-relative">
            <div
              className="team-track d-flex overflow-auto"
              ref={trackRef}
            >
              {doctors &&
              doctors.length >
                0 ? (
                doctors.map(
                  (d) => (
                    <div
                      key={
                        d.id
                      }
                      className="team-card card text-center me-3 flex-shrink-0"
                      style={{
                        width:
                          240,
                      }}
                    >
                      <div className="oval">
                        <img
                          src={
                            d.image ||
                            'https://placehold.co/240x240?text=Doctor'
                          }
                          alt={
                            d.name
                          }
                          className="img-fluid rounded-circle p-2"
                        />
                      </div>

                      <div className="card-body">
                        <h6 className="fw-bold">
                          {
                            d.name
                          }
                        </h6>

                        <p className="text-secondary small mb-0">
                          {
                            d.title
                          }
                        </p>
                      </div>
                    </div>
                  )
                )
              ) : (
                <div className="team-card card text-center">
                  <div className="oval">
                    <img
                      src="https://placehold.co/240x240?text=Doctor"
                      alt="Doctor"
                    />
                  </div>

                  <div className="card-body">
                    <h6 className="fw-bold">
                      Đang cập nhật
                    </h6>

                    <p className="text-secondary small mb-0">
                      Thông tin bác sĩ sẽ được bổ sung
                    </p>
                  </div>
                </div>
              )}
            </div>

            <button
              className="team-nav team-prev btn btn-secondary position-absolute start-0 top-50 translate-middle-y"
              aria-label="Trước"
              onClick={() =>
                handleScroll(
                  'left'
                )
              }
            >
              <i className="fa-solid fa-angle-left" />
            </button>

            <button
              className="team-nav team-next btn btn-secondary position-absolute end-0 top-50 translate-middle-y"
              aria-label="Sau"
              onClick={() =>
                handleScroll(
                  'right'
                )
              }
            >
              <i className="fa-solid fa-angle-right" />
            </button>
          </div>
        </div>

        <div className="text-center mt-4">
          <Link
            to="/bac-si"
            className="btn btn-primary"
            style={{
              backgroundColor:
                'var(--primary)',

              borderColor:
                'var(--primary)',
            }}
          >
            Chi tiết
          </Link>
        </div>
      </section>

      <section className="py-5">
        <div className="container">
          <div className="row g-4 align-items-center">
            <div className="col-lg-6 order-lg-1 order-2">
              <h3 className="fw-bold lh-base">
                Lý do nên chọn{' '}

                <span
                  className="text-primary"
                  style={{
                    color:
                      'var(--primary)',
                  }}
                >
                  BIO MEDIC CENTER
                </span>{' '}
                ?
              </h3>

              <ul className="bmc-check mt-3 list-unstyled">
                <li className="mb-2">
                  <i className="fa-solid fa-check text-primary me-2" />
                  Lấy và trả kết quả xét nghiệm tiện
                  lợi, nhanh chóng
                </li>

                <li className="mb-2">
                  <i className="fa-solid fa-check text-primary me-2" />
                  Đội ngũ chuyên gia y tế hàng đầu
                  Việt Nam
                </li>

                <li className="mb-2">
                  <i className="fa-solid fa-check text-primary me-2" />
                  Chi phí xét nghiệm hợp lý
                </li>

                <li className="mb-2">
                  <i className="fa-solid fa-check text-primary me-2" />
                  Sẵn sàng hỗ trợ bệnh nhân từ chuyên
                  gia y tế
                </li>

                <li className="mb-2">
                  <i className="fa-solid fa-check text-primary me-2" />
                  Máy móc xét nghiệm tiên tiến hàng
                  đầu thế giới
                </li>
              </ul>
            </div>

            <div className="col-lg-6 order-lg-2 order-1">
              <img
                src="https://nganngan5905-tech.github.io/LTWEB/snapedit_1758437422060.jpeg"
                className="img-fluid rounded-4 w-100"
                alt="Lý do chọn"
              />
            </div>
          </div>
        </div>
      </section>

      <section className="stats-strip py-5 text-white">
        <div
          className="container p-4 rounded"
          style={{
            backgroundColor:
              'var(--primary, #0B63E5)',
          }}
        >
          <div className="row text-center">
            <div className="col-6 col-lg-3 mb-3 mb-lg-0">
              <div className="stat-num fs-2 fw-bold">
                100.000+
              </div>

              <div>
                Khách hàng mỗi năm
              </div>
            </div>

            <div className="col-6 col-lg-3 mb-3 mb-lg-0">
              <div className="stat-num fs-2 fw-bold">
                10+
              </div>

              <div>
                Chuyên gia y tế
              </div>
            </div>

            <div className="col-6 col-lg-3">
              <div className="stat-num fs-2 fw-bold">
                30+
              </div>

              <div>
                Dịch vụ xét nghiệm
              </div>
            </div>

            <div className="col-6 col-lg-3">
              <div className="stat-num fs-2 fw-bold">
                1
              </div>

              <div>
                Cơ sở khám chữa bệnh
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="py-5">
        <div className="container">
          <div className="row g-4 align-items-center">
            <div className="col-lg-6">
              <h2 className="section-title text-start">
                Chứng chỉ
              </h2>

              <p className="lead">
                Sao Khuê là giải thưởng danh giá… Năm
                2023, Bio Medic Center được vinh danh
                là giải pháp xuất sắc thuộc lĩnh vực y
                tế, chăm sóc sức khỏe.
              </p>
            </div>

            <div className="col-lg-6">
              <img
                src="https://nganngan5905-tech.github.io/LTWEB/snapedit_1758440530650.jpeg"
                className="img-fluid rounded-4"
                alt="Chứng chỉ"
              />
            </div>
          </div>
        </div>
      </section>

      <section
        id="services"
        className="py-5 bg-light"
      >
        <div className="container">
          <h2 className="section-title">
            Dịch vụ xét nghiệm của BIO Medic Center
          </h2>

          <p className="section-subtitle">
            Cung cấp đa dạng các loại xét nghiệm từ
            cơ bản đến chuyên sâu với độ chính xác cao
          </p>

          <div className="row g-4 mt-1">
            {services.map(
              (item) => (
                <div
                  key={
                    item.id
                  }
                  className="col-md-6 col-lg-4"
                >
                  <div className="card service h-100 shadow-sm border-0">
                    <img
                      src={
                        item.image
                      }
                      className="card-img-top"
                      alt={
                        item.title
                      }
                      style={{
                        height:
                          '200px',

                        objectFit:
                          'cover',
                      }}
                    />

                    <div className="card-body">
                      <h6 className="fw-bold mb-1">
                        {
                          item.title
                        }
                      </h6>

                      <p className="small text-secondary mb-2">
                        {
                          item.desc
                        }
                      </p>

                      <div className="price">
                        <span className="now fw-bold text-danger me-2">
                          {
                            item.priceNow
                          }
                        </span>

                        <span className="old text-muted text-decoration-line-through">
                          {
                            item.priceOld
                          }
                        </span>
                      </div>
                    </div>
                  </div>
                </div>
              )
            )}
          </div>

          <div className="text-center mt-4">
            <Link
              to="/dich-vu"
              className="btn btn-primary"
              style={{
                backgroundColor:
                  'var(--primary)',

                borderColor:
                  'var(--primary)',
              }}
            >
              Xem thêm
            </Link>
          </div>
        </div>
      </section>

      <section
        id="news"
        className="py-5 bg-light"
      >
        <div className="container">
          <h2 className="section-title">
            Tin tức y khoa
          </h2>

          <p className="section-subtitle">
            Cập nhật những thông tin mới nhất về y tế,
            sức khỏe và các nghiên cứu khoa học
          </p>

          <div className="row g-4 mt-1">
            {news.map(
              (item) => (
                <div
                  key={
                    item.id
                  }
                  className="col-md-4"
                >
                  <div className="card shadow-sm h-100 news border-0">
                    <Link
                      to={`/tin-tuc/${item.id}`}
                    >
                      <img
                        src={
                          item.image
                        }
                        className="card-img-top"
                        alt={
                          item.title
                        }
                        style={{
                          height:
                            '200px',

                          objectFit:
                            'cover',
                        }}
                      />
                    </Link>

                    <div className="card-body">
                      <h6 className="fw-bold">
                        <Link
                          className="text-decoration-none text-dark"
                          to={`/tin-tuc/${item.id}`}
                        >
                          {
                            item.title
                          }
                        </Link>
                      </h6>

                      <p className="small text-secondary mb-0">
                        {
                          item.content
                        }
                      </p>
                    </div>
                  </div>
                </div>
              )
            )}
          </div>

          <div className="text-center mt-4">
            <Link
              to="/tin-tuc"
              className="btn btn-primary"
              style={{
                backgroundColor:
                  'var(--primary)',

                borderColor:
                  'var(--primary)',
              }}
            >
              Xem thêm
            </Link>
          </div>
        </div>
      </section>

      <section
        id="contact"
        className="contact py-5"
      >
        <div className="container">
          <div className="row g-4 align-items-center">
            <div className="col-lg-6">
              <div className="card p-4 shadow-sm contact-card">
                <h3
                  className="fw-bold text-primary"
                  style={{
                    color:
                      'var(--primary)',
                  }}
                >
                  Liên hệ với chúng tôi!
                </h3>

                <p className="text-secondary">
                  Để lại thông tin, chúng tôi sẽ liên
                  hệ tư vấn miễn phí cho bạn
                </p>

                <form
                  className="mt-3"
                  onSubmit={
                    handleSubmit
                  }
                >
                  <div className="mb-3">
                    <label className="form-label">
                      Họ và tên
                    </label>

                    <input
                      type="text"
                      name="fullName"
                      value={
                        formData.fullName
                      }
                      onChange={
                        handleInputChange
                      }
                      className="form-control form-control-lg"
                      placeholder="Nhập họ và tên"
                      required
                    />
                  </div>

                  <div className="mb-3">
                    <label className="form-label">
                      Số điện thoại
                    </label>

                    <input
                      type="tel"
                      name="phone"
                      value={
                        formData.phone
                      }
                      onChange={
                        handleInputChange
                      }
                      className="form-control form-control-lg"
                      placeholder="Nhập số điện thoại"
                      required
                    />
                  </div>

                  <div className="mb-3">
                    <label className="form-label">
                      Dịch vụ quan tâm
                    </label>

                    <select
                      name="serviceId"
                      value={
                        formData.serviceId
                      }
                      onChange={
                        handleInputChange
                      }
                      className="form-select form-control-lg"
                    >
                      <option value="">
                        Chọn dịch vụ
                      </option>

                      <option value="1">
                        Xét nghiệm nước tiểu
                      </option>

                      <option value="2">
                        Xét nghiệm thận - chức năng
                        thận
                      </option>

                      <option value="3">
                        Xét nghiệm gan mật - chức năng
                        gan
                      </option>

                      <option value="4">
                        Xét nghiệm gan mật - viêm gan
                        Virus
                      </option>

                      <option value="5">
                        Xét nghiệm ung bướu - dấu ấn
                        khối u
                      </option>

                      <option value="6">
                        Xét nghiệm huyết học - công
                        thức máu
                      </option>

                      <option value="7">
                        Xét nghiệm huyết học - đông máu
                      </option>

                      <option value="8">
                        Xét nghiệm tim mạch - rối loạn
                        lipid máu (cơ bản)
                      </option>

                      <option value="9">
                        Xét nghiệm tim mạch - rối loạn
                        lipid máu (chuyên sâu)
                      </option>

                      <option value="10">
                        Xét nghiệm nội tiết - chuyển hóa
                        đường
                      </option>

                      <option value="11">
                        Xét nghiệm nội tiết - tuyến giáp
                      </option>

                      <option value="12">
                        Xét nghiệm nội thận - khoáng chất
                        và điện giải
                      </option>

                      <option value="13">
                        Xét nghiệm miễn dịch - dị ứng
                      </option>

                      <option value="14">
                        Xét nghiệm cao cấp - di truyền
                      </option>
                    </select>
                  </div>

                  <button
                    type="submit"
                    className="btn btn-primary w-100 py-2 fw-semibold"
                    style={{
                      backgroundColor:
                        'var(--primary)',

                      borderColor:
                        'var(--primary)',
                    }}
                  >
                    Gửi thông tin
                  </button>
                </form>
              </div>
            </div>

            <div className="col-lg-6 text-center">
              <img
                src="https://nganngan5905-tech.github.io/LTWEB/snapedit_1758442364049-removebg-preview.png"
                className="img-fluid rounded-4"
                alt="Liên hệ"
              />
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}